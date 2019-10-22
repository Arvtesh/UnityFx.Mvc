// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	using Debug = UnityEngine.Debug;

	/// <summary>
	/// 
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IViewController"/>
	public class Presenter : MonoBehaviour, IPresenter, ICommandTarget, IDisposable
	{
		#region data

		private IServiceProvider _serviceProvider;
		private IViewFactory _viewFactory;

		private LinkedList<IPresentable> _presentables = new LinkedList<IPresentable>();
		private ViewControllerCollection _controllers;

		private int _idCounter;
		private int _busyCounter;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// A read-only stack of <see cref="IViewController"/> objects.
		/// </summary>
		public class ViewControllerCollection : IReadOnlyCollection<IViewController>
		{
			private readonly LinkedList<IPresentable> _presentables;

			internal ViewControllerCollection(LinkedList<IPresentable> presentables)
			{
				_presentables = presentables;
			}

			/// <summary>
			/// Gets top element of the stack.
			/// </summary>
			/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
			public IViewController Peek()
			{
				if (_presentables.First is null)
				{
					throw new InvalidOperationException();
				}

				return _presentables.Last.Value.Controller;
			}

			/// <summary>
			/// Attempts to get top controller of the collection.
			/// </summary>
			public bool TryPeek(out IViewController controller)
			{
				controller = _presentables.Last?.Value.Controller;
				return controller != null;
			}

			/// <summary>
			/// Gets number of controllers in the collection.
			/// </summary>
			public int Count => _presentables.Count;

			/// <summary>
			/// Enumerates all controllers in the collection starting with the top (last) one.
			/// </summary>
			public IEnumerator<IViewController> GetEnumerator()
			{
				var p = _presentables.Last;

				while (p != null)
				{
					yield return p.Value.Controller;
					p = p.Previous;
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		/// <summary>
		/// Gets a read-only controller collection.
		/// </summary>
		public ViewControllerCollection Controllers => _controllers;

		/// <summary>
		/// Gets an active controller (or <see langword="null"/>).
		/// </summary>
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

		/// <summary>
		/// Gets a <see cref="IServiceProvider"/> used to resolve controller dependencies.
		/// </summary>
		protected internal IServiceProvider ServiceProvider => _serviceProvider;

		/// <summary>
		/// Gets a value indicating whether the instance is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="Presenter"/> class.
		/// </summary>
		/// <param name="serviceProvider">A <see cref="IServiceProvider"/> used to resolve controller dependencies.</param>
		/// <param name="viewFactory">A <see cref="IViewFactory"/> that is used to create views.</param>
		public void Initialize(IServiceProvider serviceProvider, IViewFactory viewFactory)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
			_controllers = new ViewControllerCollection(_presentables);
		}

		/// <summary>
		/// Throws <see cref="ObjectDisposedException"/> if the instance is disposed.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="IsDisposed"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region virtual interface

		/// <summary>
		/// Called on each frame. Default implementation does nothing.
		/// </summary>
		protected virtual void OnUpdate(float frameTime)
		{
		}

		/// <summary>
		/// Called when the instance is disposed. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="ThrowIfDisposed"/>
		/// <seealso cref="IsDisposed"/>
		protected virtual void OnDispose()
		{
		}

		#endregion

		#region internals

		internal IPresentResult PresentAsync(IPresentable presentable, Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfBusy();
			ThrowIfInvalidControllerType(controllerType);

			var result = CreatePresentable(presentable, controllerType, args);
			PresentInternal(result);
			return result;
		}

		internal void Dismiss(IPresentable presentable)
		{
			// 1st pass: dismiss child nodes.
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

			// 2nd pass: dispose child nodes.
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

		internal void Remove(IPresentable presentable)
		{
			_presentables.Remove(presentable);
		}

		internal int GetNextId()
		{
			return ++_idCounter;
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
				node.Value.Update(frameTime, node.Value == topPresentable);
				node = node.Next;
			}

			OnUpdate(frameTime);
		}

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IPresenter

		public IPresentResult PresentAsync(Type controllerType, PresentArgs args)
		{
			return PresentAsync(null, controllerType, args);
		}

		#endregion

		#region ICommandTarget

		/// <summary>
		/// Invokes a command on the controllers presented.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>Returns <see langword="true"/> if the command has been handled; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand(string commandName, object args)
		{
			ThrowIfDisposed();

			if (!string.IsNullOrEmpty(commandName))
			{
				var node = _presentables.Last;

				while (node != null)
				{
					var p = node.Value;

					if (p.InvokeCommand(commandName, args))
					{
						return true;
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

				try
				{
					OnDispose();
				}
				finally
				{
					DisposeInternal();
				}
			}
		}

		#endregion

		#region implementation

		private void PresentInternal(IPresentable presentable)
		{
			var insertAfterIndex = -1;

			foreach (var p in _presentables)
			{
				if (p == presentable)
				{
					break;
				}

				++insertAfterIndex;
			}

			presentable.PresentAsync(_viewFactory, insertAfterIndex);
		}

		private IPresentable CreatePresentable(IPresentable parent, Type controllerType, PresentArgs args)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			// Resolve result type from the type of the controller.
			var resultType = GetControllerResultType(controllerType);

			if (resultType is null)
			{
				resultType = typeof(int);
			}

			// Make sure args are valid.
			if (args is null)
			{
				args = PresentArgs.Default;
			}

			// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-examine-and-instantiate-generic-types-with-reflection
			var presentResultType = typeof(PresentResult<,>).MakeGenericType(controllerType, resultType);
			var c = (IPresentable)Activator.CreateInstance(presentResultType, this, parent, controllerType, args);

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
				// Insert the presentable right after the last one wit hthe same parent.
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

		private void UpdateActiveController()
		{
			var topPresentable = _presentables.Last?.Value;

			if (topPresentable != null && !topPresentable.IsActive)
			{
				
			}
		}

		private Type GetControllerResultType(Type controllerType)
		{
			// Types inherited from ViewController<> do not require ViewControllerAttribute.
			if (typeof(ViewController<,>).IsAssignableFrom(controllerType))
			{
				return controllerType.GenericTypeArguments[1];
			}
			else
			{
				var attrs = (ViewControllerAttribute[])controllerType.GetCustomAttributes(typeof(ViewControllerAttribute), false);

				if (attrs != null && attrs.Length > 0)
				{
					return attrs[0].ResultType;
				}
			}

			return null;
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

		private void ThrowIfBusy()
		{
			if (_busyCounter > 0)
			{
				throw new InvalidOperationException();
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
