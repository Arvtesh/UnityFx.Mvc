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
		private readonly PresentArgs _defaultPresentArgs = new PresentArgs();

		private LinkedList<IPresentableProxy> _presentables = new LinkedList<IPresentableProxy>();
		private Dictionary<IViewController, IPresentableProxy> _controllerMap = new Dictionary<IViewController, IPresentableProxy>();
		private List<PresentDelegate> _presentDelegates;
		private Action<Exception> _errorDelegate;
		private IPresentableProxy _lastActive;

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

		IEnumerable<IPresentableProxy> IPresenterInternal.GetChildren(IPresentableProxy presentable)
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

		IPresentResult IPresenterInternal.PresentAsync(IPresentableProxy presentable, Type controllerType, PresentArgs args)
		{
			return PresentInternal(presentable, controllerType, args);
		}

		void IPresenterInternal.PresentCompleted(IPresentableProxy presentable, Exception e, bool cancelled)
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

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			return PresentInternal(null, controllerType, args);
		}

		#endregion

		#region IPresenterEvents

		public void Update()
		{
			var frameTime = Time.deltaTime;
			var node = _presentables.First;
			var newActive = default(IPresentableProxy);

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

					if ((p.CreationFlags & ViewControllerFlags.Exclusive) != 0)
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

		private IPresentResult PresentInternal(IPresentableProxy parent, Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			if (args is null)
			{
				args = _defaultPresentArgs;
			}

			var result = CreatePresentable(parent, controllerType, args);
			PresentInternal(result, parent, args);
			return result;
		}

		private async void PresentInternal(IPresentableProxy presentable, IPresentableProxy presentableParent, PresentArgs presentArgs)
		{
			try
			{
				// 1) Execute the middleware chain. Order is important here.
				if (_presentDelegates != null)
				{
					foreach (var middleware in _presentDelegates)
					{
						await middleware(this, presentable, presentArgs);
					}
				}

				// 2) Present the controller.
				await presentable.PresentAsyc(presentArgs);
				_controllerMap.Add(presentable.Controller, presentable);

				// 4) Dismiss the specified controllers if requested.
				if ((presentArgs.PresentOptions & PresentOptions.DismissAll) != 0)
				{
					foreach (var p in _presentables)
					{
						if (p != presentable)
						{
							p.DismissCancel();
						}
					}
				}
				else if ((presentArgs.PresentOptions & PresentOptions.DismissCurrent) != 0)
				{
					presentableParent?.DismissCancel();
				}

				// 5) Dismiss controllers of the same type if requested (for singleton controllers only).
				if ((presentable.CreationFlags & ViewControllerFlags.Singleton) != 0)
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

		private IPresentableProxy CreatePresentable(IPresentableProxy parent, Type controllerType, PresentArgs args)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			var presentContext = new PresentResultArgs()
			{
				Id = ++_idCounter,
				ServiceProvider = _serviceProvider,
				ControllerFactory = _controllerFactory,
				ControllerType = controllerType,
				ViewFactory = _viewFactory,
				ViewType = typeof(IView),
				ResultType = typeof(int)
			};

			var attrs = (ViewControllerAttribute[])controllerType.GetCustomAttributes(typeof(ViewControllerAttribute), false);

			if (attrs != null && attrs.Length > 0)
			{
				var attr = attrs[0];

				presentContext.ViewResourceId = attr.ViewResourceId;
				presentContext.CreationFlags = attr.Flags;
				presentContext.Layer = attr.Layer;
				presentContext.Tag = attr.Tag;

				if ((attr.Flags & ViewControllerFlags.AllowMultipleInstances) == 0)
				{
					ThrowIfControllerPresented(controllerType, _presentables);
				}
			}
			else
			{
				ThrowIfControllerPresented(controllerType, _presentables);
			}

			if (string.IsNullOrEmpty(presentContext.ViewResourceId))
			{
				presentContext.ViewResourceId = _controllerBindings.GetViewResourceId(controllerType);
			}

			// Types inherited from IViewControllerResult<> use specific result values.
			if (MvcUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerResult<>), out var resultType))
			{
				presentContext.ResultType = resultType.GenericTypeArguments[0];
			}

			// Types inherited from IViewControllerView<> have view type restrictions.
			if (MvcUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerView<>), out var viewType))
			{
				presentContext.ViewType = viewType.GenericTypeArguments[0];
			}

			// For child controller save parent reference.
			if ((args.PresentOptions & PresentOptions.Child) != 0)
			{
				if ((args.PresentOptions & PresentOptions.DismissCurrent) != 0)
				{
					presentContext.Parent = parent?.Parent;
				}
				else
				{
					presentContext.Parent = parent;
				}
			}

			// Instantiate the presentable.
			// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-examine-and-instantiate-generic-types-with-reflection
			var presentResultType = typeof(PresentResult<>).MakeGenericType(presentContext.ResultType);
			var c = (IPresentableProxy)Activator.CreateInstance(presentResultType, this, presentContext);

			AddPresentable(c);

			return c;
		}

		private void AddPresentable(IPresentableProxy presentable)
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

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		private static void ThrowIfControllerPresented(Type controllerType, LinkedList<IPresentableProxy> presentables)
		{
			foreach (var p in presentables)
			{
				if (p.ControllerType == controllerType)
				{
					throw new PresentException(controllerType, Messages.Format_ControllerAlreadyPresented(controllerType));
				}
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
				throw new ArgumentException(Messages.Format_ControllerTypeIsAbstract(controllerType), nameof(controllerType));
			}

			if (!typeof(IViewController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException(Messages.Format_ControllerTypeIsNotController(controllerType), nameof(controllerType));
			}
		}

		#endregion
	}
}
