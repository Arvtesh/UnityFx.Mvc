// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
#if !NET35
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A present service implementation (<see cref="IPresentService"/>).
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IViewController"/>
	public class PresentService : IPresentService
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly TreeListCollection<PresentableProxy> _controllers = new TreeListCollection<PresentableProxy>();
		private readonly PresentableStack _stack;

		private int _idCounter;
		private int _opCounter;
		private int _busyCounter;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentService"/> class.
		/// </summary>
		/// <param name="serviceProvider"></param>
		public PresentService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_stack = new PresentableStack(_controllers);
		}

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					foreach (var c in _controllers.Reverse())
					{
						c.Dismiss();
					}

					_controllers.Clear();
				}
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region internals

		internal IPresentResult Present(PresentableProxy parent, Type controllerType, PresentArgs args)
		{
			Debug.Assert(args != null);

			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);
			ThrowIfBusy();

			var result = CreateController(parent, controllerType, args);
			PresentInternal(result, args);
			return result;
		}

		internal IPresentResult<T> Present<T>(PresentableProxy parent, PresentArgs args) where T : class, IPresentable
		{
			Debug.Assert(args != null);

			ThrowIfDisposed();
			ThrowIfInvalidControllerType(typeof(T));
			ThrowIfBusy();

			var result = CreateController<T>(parent, args);
			PresentInternal(result, args);
			return result;
		}

		internal void Dismiss(PresentableProxy controller)
		{
			Debug.Assert(controller != null);

			ThrowIfDisposed();
			ThrowIfBusy();

			try
			{
				SetBusy(true);
				DismissInternal(controller);

				if (_controllers.TryPeek(out var topController))
				{
					topController.TryActivate();
				}
			}
			finally
			{
				SetBusy(false);
			}
		}

		#endregion

		#region IPresentService

		/// <inheritdoc/>
		public IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
		}

		/// <inheritdoc/>
		public IPresentable ActiveController
		{
			get
			{
				var result = _controllers.Last;

				if (result != null && result.IsActive)
				{
					return result.Controller;
				}

				return null;
			}
		}

		/// <inheritdoc/>
		public IPresentableStack Controllers => _stack;

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IPresentResult Present(Type controllerType)
		{
			return Present(null, controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			return Present(null, controllerType, args);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>() where TController : class, IPresentable
		{
			return Present<TController>(null, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IPresentable
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			return Present<TController>(null, args);
		}

		#endregion

		#region ICommandTarget

		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand(string commandName, object args)
		{
			ThrowIfDisposed();

			if (commandName == null)
			{
				throw new ArgumentNullException(nameof(commandName));
			}

			foreach (var controllerProxy in _controllers.Reverse())
			{
				if (controllerProxy.InvokeCommand(commandName, args))
				{
					return true;
				}
				else if (controllerProxy.IsModal)
				{
					// Do not forward commands past first modal controller in the stack.
					break;
				}
			}

			return false;
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private PresentableProxy CreateController(PresentableProxy parent, Type controllerType, PresentArgs args)
		{
			PresentableProxy c = null;

			try
			{
				// 1) Lock present/dismiss operations.
				SetBusy(true);

				// 2) Create a new controller. Note that while the lock is alive no other present/dismiss operation can start.
				c = new PresentableProxy(this, parent, controllerType, args, ++_idCounter);

				// 3) Add the new created controller to the stack and turn off the operations lock.
				AddController(c);
			}
			catch
			{
				DisposeInternal(c);
				throw;
			}
			finally
			{
				SetBusy(false);
			}

			return c;
		}

		private PresentableProxy<T> CreateController<T>(PresentableProxy parent, PresentArgs args) where T : class, IPresentable
		{
			PresentableProxy<T> c = null;

			try
			{
				// 1) Lock present/dismiss operations.
				SetBusy(true);

				// 2) Create a new controller. Note that while the lock is alive no other present/dismiss operation can start.
				c = new PresentableProxy<T>(this, parent, typeof(T), args, ++_idCounter);

				// 3) Add the new created controller to the stack and turn off the operations lock.
				AddController(c);
			}
			catch
			{
				DisposeInternal(c);
				throw;
			}
			finally
			{
				SetBusy(false);
			}

			return c;
		}

		private void PresentInternal(PresentableProxy controller, PresentArgs args)
		{
			try
			{
				++_opCounter;

				// Deactivate currently active controller (if needed).
				if (_controllers.TryPeek(out var topController))
				{
					if (controller.Parent != null)
					{
						if (topController.IsChildOf(controller.Parent))
						{
							topController.TryDeactivate();
						}
					}
					else
					{
						topController.TryDeactivate();
					}
				}

				// Present the new one.
				controller.OnPresent();

				// If this is the last operation, activate the controller.
				if (_opCounter == 1)
				{
					if (controller == _controllers.Last)
					{
						if ((args.Options & PresentOptions.DoNotActivate) == 0)
						{
							controller.TryActivate();
						}
					}
					else if (_controllers.TryPeek(out topController))
					{
						topController.TryActivate();
					}
				}
			}
			catch
			{
				DismissInternal(controller);
				throw;
			}
			finally
			{
				--_opCounter;
			}
		}

		private void DismissInternal(PresentableProxy controller)
		{
			try
			{
				++_opCounter;

				controller.TryDeactivate();
				controller.TryDismiss();

				// If this is the last operation, activate the controller.
				if (_opCounter == 1 && _controllers.TryPeek(out var topController))
				{
					topController.TryActivate();
				}
			}
			finally
			{
				--_opCounter;

				DisposeInternal(controller);
			}
		}

		private void DisposeInternal(PresentableProxy controller)
		{
			if (controller != null)
			{
				_controllers.Remove(controller);

				try
				{
					SetBusy(true);
					controller.Dispose();
				}
				finally
				{
					SetBusy(false);
				}
			}
		}

		private void AddController(PresentableProxy controller)
		{
			Debug.Assert(controller != null);
			Debug.Assert(!_disposed);

			var parent = controller.Parent;

			if (parent != null)
			{
				var prev = parent;
				var next = parent.Next;

				while (next != null && next.IsChildOf(parent))
				{
					prev = next;
					next = prev.Next;
				}

				_controllers.Add(controller, prev);
			}
			else
			{
				_controllers.Add(controller);
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

		private void ThrowIfInvalidArgs(PresentArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
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
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (controllerType.IsAbstract)
			{
				throw new ArgumentException($"Cannot instantiate abstract type {controllerType.Name}", nameof(controllerType));
			}

			if (!typeof(IViewController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException($"A state controller is expected to implement " + typeof(IViewController).Name, nameof(controllerType));
			}
		}

		#endregion
	}
}
