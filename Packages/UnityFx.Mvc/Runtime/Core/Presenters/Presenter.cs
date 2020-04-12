// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Implementation of <see cref="IPresentService"/>.
	/// </summary>
	/// <seealso cref="PresenterBuilder"/>
	internal sealed partial class Presenter : IPresentService, IPresenterEvents, IPresenterInternal
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly IViewFactory _viewFactory;
		private readonly IViewControllerFactory _controllerFactory;
		private readonly IViewControllerBindings _controllerBindings;
		private readonly IPresenterEventSource _eventSource;
		private readonly ViewControllerCollection _controllers;

		private LinkedList<IPresentable> _presentables = new LinkedList<IPresentable>();
		private Dictionary<IViewController, IPresentable> _controllerMap = new Dictionary<IViewController, IPresentable>();
		private List<PresentDelegate> _presentDelegates;
		private Action<Exception> _errorDelegate;
		private IPresentable _lastActive;

		private int _idCounter;
		private bool _disposed;

		#endregion

		#region interface

		internal bool NeedEventSource => _eventSource is null;

		internal Presenter(IServiceProvider serviceProvider, IViewFactory viewFactory, IViewControllerFactory controllerFactory, IViewControllerBindings controllerBindings, IPresenterEventSource eventSource)
		{
			Debug.Assert(serviceProvider != null);
			Debug.Assert(viewFactory != null);
			Debug.Assert(controllerFactory != null);
			Debug.Assert(controllerBindings != null);

			_serviceProvider = serviceProvider;
			_viewFactory = viewFactory;
			_controllerFactory = controllerFactory;
			_controllerBindings = controllerBindings;
			_eventSource = eventSource;
			_controllers = new ViewControllerCollection(_presentables);

			_eventSource?.AddPresenter(this);
		}

		internal void SetMiddleware(List<PresentDelegate> middleware)
		{
			_presentDelegates = middleware;
		}

		internal void SetErrorHandler(Action<Exception> errorDelegate)
		{
			_errorDelegate = errorDelegate;
		}

		#endregion

		#region IPresenterInternal

		IEnumerable<IPresentable> IPresenterInternal.GetChildren(IPresentable presentable)
		{
			var node = _presentables.Find(presentable)?.Next;

			while (node != null)
			{
				var p = node.Value;

				if (p.Parent == presentable)
				{
					yield return p;
				}

				node = node.Next;
			}
		}

		IPresentResult IPresenterInternal.PresentAsync(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args)
		{
			return PresentInternal(presentable, controllerType, presentOptions, parent, args);
		}

		void IPresenterInternal.PresentCompleted(IPresentable presentable, Exception e, bool cancelled)
		{
			if (!_disposed)
			{
				PresentCompleted?.Invoke(this, new PresentCompletedEventArgs(presentable, e, cancelled));
			}
		}

		void IPresenterInternal.ReportError(Exception e)
		{
			if (!_disposed)
			{
				_errorDelegate?.Invoke(e);
			}
		}

		#endregion

		#region IPresentService

		public event EventHandler<PresentCompletedEventArgs> PresentCompleted;

		public IServiceProvider ServiceProvider => _serviceProvider;

		public IViewFactory ViewFactory => _viewFactory;

		public IViewControllerCollection Controllers => _controllers;

		public IViewController ActiveController => _lastActive?.Controller;

		public bool TryGetInfo(IViewController controller, out IPresentInfo info)
		{
			if (_controllerMap.TryGetValue(controller, out var result))
			{
				info = result;
				return true;
			}

			info = null;
			return false;
		}

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType, PresentArgs args, PresentOptions presentOptions, Transform parent)
		{
			return PresentInternal(null, controllerType, presentOptions, parent, args);
		}

		#endregion

		#region IPresenterEvents

		public void Update()
		{
			var frameTime = Time.deltaTime;
			var node = _presentables.First;
			var newActive = default(IPresentable);

			// 1) Remove dismissed controllers & find active.
			while (node != null)
			{
				var p = node.Value;
				node = node.Next;

				if (p.IsDismissed)
				{
					if (p.Controller != null)
					{
						_controllerMap.Remove(p.Controller);
					}

					_presentables.Remove(p);
				}
				else
				{
					newActive = p;
				}
			}

			// 2) Activate/deactivate.
			if (newActive != _lastActive && newActive != null && newActive.TryActivate())
			{
				_lastActive?.Deactivate();
				_lastActive = newActive;
			}

			// 3) Update presentables.
			node = _presentables.First;

			while (node != null)
			{
				node.Value.Update(frameTime);
				node = node.Next;
			}
		}

		#endregion

		#region ICommandTarget

		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="command">Command to invoke.</param>
		/// <returns>Returns <see langword="true"/> if the command has been handled; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand(Command command, Variant args)
		{
			if (!command.IsNull && !_disposed)
			{
				var node = _presentables.Last;

				while (node != null)
				{
					var p = node.Value;

					if (p.InvokeCommand(command, args))
					{
						return true;
					}

					if ((p.PresentOptions & PresentOptions.Exclusive) != 0)
					{
						return false;
					}

					node = node.Previous;
				}
			}

			return false;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_eventSource?.RemovePresenter(this);

				DismissPresentables();
			}
		}

		#endregion

		#region implementation

		private IPresentResult PresentInternal(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform transform, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var result = CreatePresentable(presentable, controllerType, presentOptions, args);
			PresentInternal(result, presentable, transform);
			return result;
		}

		private async void PresentInternal(IPresentable presentable, IPresentable presentableParent, Transform transform)
		{
			var zIndex = GetZIndex(presentable);

			try
			{
				// 1) Execute the middleware chain. Order is important here.
				if (_presentDelegates != null)
				{
					foreach (var middleware in _presentDelegates)
					{
						await middleware(this, presentable);
					}
				}

				// 2) Load the controller view.
				var view = await _viewFactory.CreateViewAsync(presentable.PrefabPath, presentable.Layer, zIndex, presentable.PresentOptions, transform);

				if (view is null)
				{
					throw new PresentException(presentable, "View is null.");
				}

				// 3) Create the controller (or dispose view if the controller is dismissed at this point).
				if (presentable.IsDismissed)
				{
					view.Dispose();
					throw new OperationCanceledException();
				}
				else
				{
					presentable.CreateController(view);
				}

				_controllerMap.Add(presentable.Controller, presentable);

				// 4) Dismiss the specified controllers if requested.
				if ((presentable.PresentOptions & PresentOptions.DismissAll) != 0)
				{
					foreach (var p in _presentables)
					{
						if (p != presentable)
						{
							p.DismissCancel();
						}
					}
				}
				else if ((presentable.PresentOptions & PresentOptions.DismissCurrent) != 0)
				{
					presentableParent?.DismissCancel();
				}

				// 5) Dismiss controllers of the same type if requested (for singleton controllers only).
				if ((presentable.PresentOptions & PresentOptions.Singleton) != 0)
				{
					foreach (var p in _presentables)
					{
						if (p != presentable && p.ControllerType == presentable.ControllerType)
						{
							p.DismissCancel();
						}
					}
				}
			}
			catch (Exception e)
			{
				presentable.Dismiss(e);
			}
		}

		private IPresentable CreatePresentable(IPresentable parent, Type controllerType, PresentOptions presentOptions, PresentArgs args)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			var resultType = typeof(int);
			var attrs = (ViewControllerAttribute[])controllerType.GetCustomAttributes(typeof(ViewControllerAttribute), false);
			var presentContext = new PresentResultArgs()
			{
				Id = ++_idCounter,
				ServiceProvider = _serviceProvider,
				ControllerFactory = _controllerFactory,
				ViewFactory = _viewFactory,
				ControllerType = controllerType,
				Parent = parent,
				PrefabPath = _controllerBindings.GetViewResourceId(controllerType),
				PresentOptions = presentOptions,
				PresentArgs = args ?? PresentArgs.Default
			};

			if (attrs != null && attrs.Length > 0)
			{
				var controllerAttr = attrs[0];

				presentContext.PresentOptions |= controllerAttr.PresentOptions;
				presentContext.Layer = controllerAttr.Layer;
				presentContext.Tag = controllerAttr.Tag;
			}

			// Types inherited from IViewControllerResult<> use specific result values.
			if (MvcUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerResult<>), out var t))
			{
				resultType = t.GenericTypeArguments[0];
			}

			// If parent is going to be dismissed, use its parent instead.
			if ((presentOptions & PresentOptions.Child) == 0)
			{
				presentContext.Parent = null;
			}
			else if ((presentOptions & PresentOptions.DismissAll) != 0)
			{
				presentContext.Parent = null;
			}
			else if ((presentOptions & PresentOptions.DismissCurrent) != 0)
			{
				presentContext.Parent = parent?.Parent;
			}

			// Instantiate the presentable.
			// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-examine-and-instantiate-generic-types-with-reflection
			var presentResultType = typeof(PresentResult<,>).MakeGenericType(controllerType, resultType);
			var c = (IPresentable)Activator.CreateInstance(presentResultType, this, presentContext);

			AddPresentable(c);

			return c;
		}

		private void AddPresentable(IPresentable presentable)
		{
			Debug.Assert(presentable != null);
			Debug.Assert(!_disposed);

			var parent = presentable.Parent;

			if (parent != null)
			{
				// Insert the presentable right after the last one with the same parent.
				var node = _presentables.Last;

				while (node != null)
				{
					if (node.Value.IsChildOf(parent))
					{
						break;
					}

					node = node.Previous;
				}

				_presentables.AddAfter(node, presentable);
			}
			else
			{
				_presentables.AddLast(presentable);
			}
		}

		private void DismissPresentables()
		{
			var node = _presentables.Last;

			while (node != null)
			{
				node.Value.DismissCancel();
				node = node.Previous;
			}
		}

		private int GetZIndex(IPresentable presentable)
		{
			var zIndex = 0;

			foreach (var p in _presentables)
			{
				if (p == presentable)
				{
					break;
				}
				else if (p.Layer == presentable.Layer)
				{
					++zIndex;
				}
			}

			return zIndex;
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		private static void ThrowIfInvalidControllerType(Type controllerType)
		{
			if (controllerType is null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (controllerType.IsAbstract)
			{
				throw new ArgumentException($"Cannot instantiate abstract type {controllerType.Name}.", nameof(controllerType));
			}

			if (!typeof(IViewController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException($"A view controller is expected to implement {typeof(IViewController).Name}.", nameof(controllerType));
			}
		}

		#endregion
	}
}
