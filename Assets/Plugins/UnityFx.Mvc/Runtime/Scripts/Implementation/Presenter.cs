// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// 
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IViewController"/>
	public class Presenter : IPresenter, ICommandTarget, IDisposable
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly LinkedList<IPresentable> _presentables = new LinkedList<IPresentable>();
		private readonly ViewControllerCollection _controllers;

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
			/// Attempts to get the top controller from the collection.
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
		public Presenter(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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
		/// Called when the instance is disposed.
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
			throw new NotImplementedException();
		}

		internal IPresentResult<TController> PresentAsync<TController>(IPresentable presentable, PresentArgs args) where TController : IViewController
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		internal IPresentResult<TController, TResult> PresentAsync<TController, TResult>(IPresentable presentable, PresentArgs args) where TController : IViewController
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		internal void DismissChildren(IPresentable presentable)
		{
			// 1st pass: dismiss child nodes.
			var node = _presentables.Last;

			while (node != null)
			{
				var p = node.Value;

				if (p.IsChildOf(presentable))
				{
					p.DismissChild();
				}

				if (p == presentable)
				{
					break;
				}

				node = node.Previous;
			}

			// 2nd pass: remove the child nodes.
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
					p.DisposeChild();
					_presentables.Remove(p);
				}
			}
		}

		#endregion

		#region IPresenter

		public IPresentResult PresentAsync(Type controllerType, PresentArgs args)
		{
			throw new NotImplementedException();
		}

		public IPresentResult<TController> PresentAsync<TController>(PresentArgs args) where TController : IViewController
		{
			throw new NotImplementedException();
		}

		public IPresentResult<TController, TResult> PresentAsync<TController, TResult>(PresentArgs args) where TController : IViewController
		{
			throw new NotImplementedException();
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
				OnDispose();
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
