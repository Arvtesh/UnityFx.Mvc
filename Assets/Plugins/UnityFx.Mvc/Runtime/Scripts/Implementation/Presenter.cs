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
	public class Presenter : IPresenter, ICommandTarget, IDisposable
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly TreeListCollection<PresentResult> _controllerProxies = new TreeListCollection<PresentResult>();
		private readonly ViewControllerCollection _controllers;

		private int _idCounter;
		private int _busyCounter;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="Presenter"/> class.
		/// </summary>
		/// <param name="serviceProvider">A <see cref="IServiceProvider"/> used to resolve controller dependencies.</param>
		public Presenter(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_controllers = new ViewControllerCollection(_controllerProxies);
		}

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDispose()
		{
			try
			{
				foreach (var c in _controllerProxies.Reverse())
				{
					c.Dispose();
				}
			}
			finally
			{
				_controllerProxies.Clear();
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="OnDispose"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region internals

		internal IPresentResult Present(PresentResult parent, Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfInvalidArgs(args);
			ThrowIfInvalidControllerType(controllerType);
			ThrowIfBusy();

			var result = CreateController(parent, controllerType, args);
			PresentInternal(result, args.Options, args.Data);
			return result;
		}

		internal IPresentResult<T> Present<T>(PresentResult parent, PresentArgs args) where T : class, IViewController
		{
			ThrowIfDisposed();
			ThrowIfInvalidArgs(args);
			ThrowIfInvalidControllerType(typeof(T));
			ThrowIfBusy();

			var result = CreateController<T>(parent, args);
			PresentInternal(result, args.Options, args.Data);
			return result;
		}

		internal void Dismissed(PresentResult controller)
		{
			Debug.Assert(controller != null);

			if (!_disposed)
			{
				_controllerProxies.Remove(controller);
				TryActivateTopController();
			}
		}

		#endregion

		#region IPresentService

		/// <inheritdoc/>
		public IServiceProvider ServiceProvider => _serviceProvider;

		/// <inheritdoc/>
		public IViewController ActiveController
		{
			get
			{
				var result = _controllerProxies.Last;

				if (result != null && result.IsActive)
				{
					return result.Controller;
				}

				return null;
			}
		}

		/// <inheritdoc/>
		public IViewControllerCollection Controllers => _controllers;

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
		public IPresentResult<TController> Present<TController>() where TController : class, IViewController
		{
			return Present<TController>(null, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IViewController
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

			foreach (var controllerProxy in _controllerProxies.Reverse())
			{
				if (controllerProxy.InvokeCommand(commandName, args))
				{
					return true;
				}
				else if (controllerProxy.PresentOptions.HasFlag(PresentOptions.Modal))
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
			if (!_disposed)
			{
				_disposed = true;
				OnDispose();
			}
		}

		#endregion

		#region implementation

		private PresentResult CreateController(PresentResult parent, Type controllerType, PresentArgs args)
		{
			PresentResult c = null;

			try
			{
				SetBusy(true);

				c = new PresentResult(this, parent, controllerType, args, ++_idCounter);
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

		private PresentResult<T> CreateController<T>(PresentResult parent, PresentArgs args) where T : class, IViewController
		{
			PresentResult<T> c = null;

			try
			{
				SetBusy(true);

				c = new PresentResult<T>(this, parent, typeof(T), args, ++_idCounter);
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

		private bool TryActivateTopController()
		{
			if (_controllerProxies.TryPeek(out var topController))
			{
				return topController.TryActivate();
			}

			return false;
		}

		private void DeactivateTopControllerIfNeeded(PresentResult controller)
		{
			if (_controllerProxies.TryPeek(out var topController))
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
		}

		private void PrePresent(PresentResult controller, PresentOptions presentOptions)
		{
			if ((presentOptions & PresentOptions.DismissCurrent) != 0)
			{
				if (controller.Parent != null)
				{
					foreach (var c in controller.Parent.GetChildrenRecursive().Reverse())
					{
						if (c != controller)
						{
							DismissInternal(c);
						}
					}
				}
			}
			else if ((presentOptions & PresentOptions.DismissAll) != 0)
			{
				foreach (var c in _controllerProxies.Reverse())
				{
					if (c != controller)
					{
						DismissInternal(c);
					}
				}
			}
		}

		private void PostPresent(PresentResult controller, PresentOptions presentOptions)
		{
			// If this is the last operation, activate the controller.
			if (controller == _controllerProxies.Last)
			{
				if ((presentOptions & PresentOptions.DoNotActivate) == 0)
				{
					controller.TryActivate();
				}
			}
		}

		private async void PresentInternal(PresentResult controller, PresentOptions presentOptions, object userState)
		{
			try
			{
				DeactivateTopControllerIfNeeded(controller);
				PrePresent(controller, presentOptions);

				await controller.PresentAsync(userState);
			}
			catch
			{
				DisposeInternal(controller);
				throw;
			}

			try
			{
				PostPresent(controller, presentOptions);
			}
			catch
			{
				DismissInternal(controller);
				throw;
			}
		}

		private void DismissInternal(PresentResult controller)
		{
			try
			{
				controller.TryDeactivate();
				controller.TryDismiss();

				// If this is the last operation, activate the controller.
				if (_controllerProxies.TryPeek(out var topController))
				{
					topController.TryActivate();
				}
			}
			finally
			{
				DisposeInternal(controller);
			}
		}

		private void DisposeInternal(PresentResult controller)
		{
			if (controller != null)
			{
				_controllerProxies.Remove(controller);

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

		private void OnDismiss(object sender, EventArgs args)
		{
			DisposeInternal(sender as PresentResult);
		}

		private void AddController(PresentResult controller)
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

				_controllerProxies.Add(controller, prev);
			}
			else
			{
				_controllerProxies.Add(controller);
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
