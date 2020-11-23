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
	public sealed partial class Presenter : IPresenter, ICommandTarget, IDisposable
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly IViewFactory _viewFactory;
		private readonly IViewControllerFactory _controllerFactory;
		private readonly IViewControllerBindings _controllerBindings;
		private readonly ViewControllerCollection _controllers;
		private readonly PresentArgs _defaultPresentArgs = new PresentArgs();

		private LinkedList<PresentResult> _presentables = new LinkedList<PresentResult>();
		private Dictionary<IViewController, PresentResult> _controllerMap = new Dictionary<IViewController, PresentResult>();
		private List<PresentDelegate> _presentDelegates;
		private Action<Exception> _errorDelegate;
		private PresentResult _lastActive;

		private int _idCounter;
		private bool _disposed;

		#endregion

		#region interface

		public IServiceProvider ServiceProvider => _serviceProvider;

		public ViewControllerCollection Controllers => _controllers;

		internal IViewFactory ViewFactory => _viewFactory;

		internal IViewControllerFactory ControllerFactory => _controllerFactory;

		internal IViewControllerBindings ControllerBindings => _controllerBindings;

		internal Presenter(IServiceProvider serviceProvider, IViewFactory viewFactory, IViewControllerFactory controllerFactory, IViewControllerBindings controllerBindings)
		{
			Debug.Assert(serviceProvider != null);
			Debug.Assert(viewFactory != null);
			Debug.Assert(controllerFactory != null);
			Debug.Assert(controllerBindings != null);

			_serviceProvider = serviceProvider;
			_viewFactory = viewFactory;
			_controllerFactory = controllerFactory;
			_controllerBindings = controllerBindings;
			_controllers = new ViewControllerCollection(_presentables);
		}

		internal void SetMiddleware(List<PresentDelegate> middleware)
		{
			_presentDelegates = middleware;
		}

		internal void SetErrorHandler(Action<Exception> errorDelegate)
		{
			_errorDelegate = errorDelegate;
		}

		internal IEnumerable<PresentResult> GetChildren(PresentResult presentable)
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

		internal IPresentResult PresentAsync(PresentResult parent, Type controllerType, PresentArgs presentArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			if (presentArgs is null)
			{
				presentArgs = _defaultPresentArgs;
			}

			var result = CreatePresentable(parent, controllerType, presentArgs);
			PresentInternal(result, parent, presentArgs);
			return result;
		}

		internal void Update()
		{
			var node = _presentables.First;
			var newActive = default(PresentResult);

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
				node.Value.Update();
				node = node.Next;
			}
		}

		internal void ReportError(Exception e)
		{
			if (!_disposed)
			{
				_errorDelegate?.Invoke(e);
			}
		}

		internal void OnPresentCompleted(PresentResult presentResult)
		{
			if (!_disposed)
			{
				_presentables.Remove(presentResult);
			}
		}

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			return PresentAsync(null, controllerType, args);
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

					//if ((p.CreationFlags & ViewControllerFlags.Exclusive) != 0)
					//{
					//	return false;
					//}

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
				DismissPresentables();
			}
		}

		#endregion

		#region implementation

		private async void PresentInternal(PresentResult presentable, PresentResult presentableParent, PresentArgs presentArgs)
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
							p.Dismiss();
						}
					}
				}
				else if ((presentArgs.PresentOptions & PresentOptions.DismissCurrent) != 0)
				{
					presentableParent?.Dismiss();
				}
			}
			catch (Exception e)
			{
				presentable.Dismiss(e);
			}
		}

		private PresentResult CreatePresentable(PresentResult parent, Type controllerType, PresentArgs presentArgs)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			var presentContext = new PresentResultArgs()
			{
				Id = ++_idCounter,
				ControllerType = controllerType,
				ViewType = typeof(IView),
				ResultType = typeof(int),
				ArgsType = typeof(PresentArgs)
			};

			var attrs = (ViewControllerAttribute[])controllerType.GetCustomAttributes(typeof(ViewControllerAttribute), false);

			if (attrs != null && attrs.Length > 0)
			{
				var attr = attrs[0];

				presentContext.Queue = attr.Queue;
				presentContext.Tag = attr.Tag;
			}
			else
			{
				ThrowIfControllerPresented(controllerType, _presentables);
			}

			// Types inherited from IViewControllerResult<> use specific result values.
			if (PresentUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerResult<>), out var resultType))
			{
				presentContext.ResultType = resultType.GenericTypeArguments[0];
			}

			// Types inherited from IViewControllerView<> have view type restrictions.
			if (PresentUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerView<>), out var viewType))
			{
				presentContext.ViewType = viewType.GenericTypeArguments[0];
			}

			// Types inherited from IViewControllerArgs<> have args type restrictions.
			if (PresentUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerArgs<>), out var argsType))
			{
				if (presentArgs.GetType() != argsType)
				{
					throw new ArgumentException(Messages.Format_InvalidArgsType(argsType), nameof(presentArgs));
				}

				presentContext.ArgsType = argsType.GenericTypeArguments[0];
			}

			// For child controller save parent reference.
			//if ((presentArgs.PresentOptions & PresentOptions.Child) != 0)
			//{
			//	if ((presentArgs.PresentOptions & PresentOptions.DismissCurrent) != 0)
			//	{
			//		presentContext.Parent = parent?.Parent;
			//	}
			//	else
			//	{
			//		presentContext.Parent = parent;
			//	}
			//}

			// Instantiate the presentable.
			// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-examine-and-instantiate-generic-types-with-reflection
			var presentResultType = typeof(PresentResult<>).MakeGenericType(presentContext.ResultType);
			var c = (PresentResult)Activator.CreateInstance(presentResultType, this, presentContext);

			AddPresentable(c);

			return c;
		}

		private void AddPresentable(PresentResult presentable)
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
				node.Value.Dismiss();
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

		private static void ThrowIfControllerPresented(Type controllerType, LinkedList<PresentResult> presentables)
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
