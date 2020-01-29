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
	/// A <see cref="MonoBehaviour"/>-based presenter implementation.
	/// </summary>
	/// <seealso cref="PresenterBuilder"/>
	internal sealed partial class Presenter : MonoBehaviour, IPresentService, IPresenterInternal
	{
		#region data

		private IServiceProvider _serviceProvider;
		private IViewFactory _viewFactory;
		private IViewControllerFactory _controllerFactory;

		private LinkedList<IPresentable> _presentables = new LinkedList<IPresentable>();
		private Dictionary<IViewController, IPresentable> _controllerMap = new Dictionary<IViewController, IPresentable>();
		private List<PresentDelegate> _presentDelegates;
		private ViewControllerCollection _controllers;

		private int _idCounter;
		private int _busyCounter;
		private bool _disposed;

		#endregion

		#region interface

		internal void Initialize(IServiceProvider serviceProvider, IViewFactory viewFactory, IViewControllerFactory controllerFactory)
		{
			Debug.Assert(serviceProvider != null);
			Debug.Assert(viewFactory != null);
			Debug.Assert(controllerFactory != null);

			_serviceProvider = serviceProvider;
			_viewFactory = viewFactory;
			_controllerFactory = controllerFactory;
			_controllers = new ViewControllerCollection(_presentables);
		}

		internal void SetMiddleware(List<PresentDelegate> middleware)
		{
			_presentDelegates = middleware;
		}

		#endregion

		#region MonoBehaviour

		private void Update()
		{
			var frameTime = Time.deltaTime;
			var topPresentable = _presentables.Last?.Value;
			var node = _presentables.First;

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
					p.Update(frameTime, p == topPresentable);
				}
			}
		}

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IPresenterInternal

		IPresentResult IPresenterInternal.PresentAsync(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args)
		{
			return PresentInternal(presentable, controllerType, presentOptions, parent, args);
		}

		void IPresenterInternal.Dismiss(IPresentable presentable)
		{
			// 1) Dismiss child nodes.
			var node = _presentables.Last;

			while (node != null)
			{
				var p = node.Value;

				if (p.IsChildOf(presentable))
				{
					p.DismissUnsafe();
				}

				if (p == presentable)
				{
					break;
				}

				node = node.Previous;
			}

			// 2) Dispose child nodes.
			node = _presentables.Last;

			while (node != null)
			{
				var p = node.Value;

				if (p == presentable)
				{
					break;
				}

				node = node.Previous;

				if (p.IsChildOf(presentable))
				{
					p.DisposeUnsafe();
				}
			}
		}

		#endregion

		#region IPresentService

		public IServiceProvider ServiceProvider => _serviceProvider;

		public IViewFactory ViewFactory => _viewFactory;

		public IViewControllerCollection Controllers => _controllers;

		public IViewController ActiveController
		{
			get
			{
				var p = _presentables.Last?.Value;

				if (p != null && p.IsActive)
				{
					return p.Controller;
				}

				return default;
			}
		}

		public bool TryGetInfo(IViewController controller, out IViewControllerInfo info)
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

		#region ICommandTarget

		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="command">Command to invoke.</param>
		/// <returns>Returns <see langword="true"/> if the command has been handled; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand<TCommand>(TCommand command)
		{
			if (command != null && !_disposed)
			{
				var node = _presentables.Last;

				while (node != null)
				{
					var p = node.Value;

					if (p.InvokeCommand(command))
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
				DisposeInternal();
			}
		}

		#endregion

		#region implementation

		private IPresentResult PresentInternal(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform transform, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfBusy();
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
				var view = await _viewFactory.CreateAsync(presentable.PrefabPath, presentable.Layer, zIndex, presentable.PresentOptions, transform);

				if (view is null)
				{
					throw new InvalidOperationException();
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
							p.Dispose();
						}
					}
				}
				else if ((presentable.PresentOptions & PresentOptions.DismissCurrent) != 0)
				{
					presentableParent?.Dispose();
				}

				// 5) Dismiss controllers of the same type if requested (for singleton controllers only).
				if ((presentable.PresentOptions & PresentOptions.Singleton) != 0)
				{
					foreach (var p in _presentables)
					{
						if (p != presentable && p.ControllerType == presentable.ControllerType)
						{
							p.Dispose();
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
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
				PresentOptions = presentOptions,
				PresentArgs = args ?? PresentArgs.Default
			};

			if (attrs != null && attrs.Length > 0)
			{
				var controllerAttr = attrs[0];

				presentContext.PresentOptions |= controllerAttr.PresentOptions;
				presentContext.Layer = controllerAttr.Layer;
				presentContext.Tag = controllerAttr.Tag;
				presentContext.PrefabPath = controllerAttr.PrefabPath;
			}

			// Types inherited from IViewControllerResult<> use specific result values.
			if (IsAssignableToGenericType(controllerType, typeof(IViewControllerResult<>), out var t))
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

		private void DisposeInternal()
		{
			// 1st pass: dismiss child nodes.
			var node = _presentables.Last;

			while (node != null)
			{
				node.Value.DismissUnsafe();
				node = node.Previous;
			}

			// 2nd pass: dispose child nodes.
			node = _presentables.Last;

			while (node != null)
			{
				var p = node.Value;
				node = node.Previous;
				p.DisposeUnsafe();
			}
		}

		private void SetBusy(bool busy)
		{
			if (busy)
			{
				++_busyCounter;
			}
			else
			{
				--_busyCounter;
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

		private void ThrowIfBusy()
		{
			if (_busyCounter > 0)
			{
				throw new InvalidOperationException();
			}
		}

		private static bool IsAssignableToGenericType(Type givenType, Type genericType, out Type closedGenericType)
		{
			// NOTE: See https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types for details.
			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
				{
					closedGenericType = it;
					return true;
				}
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
			{
				closedGenericType = givenType;
				return true;
			}

			var baseType = givenType.BaseType;

			if (baseType == null)
			{
				closedGenericType = null;
				return false;
			}

			return IsAssignableToGenericType(baseType, genericType, out closedGenericType);
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
