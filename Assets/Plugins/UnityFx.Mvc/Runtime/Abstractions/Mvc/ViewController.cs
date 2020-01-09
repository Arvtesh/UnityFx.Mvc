// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Implementation of <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class ViewController : IViewController, IViewControllerEvents
	{
		#region data

		private readonly IPresentContext _context;

		#endregion

		#region interface

		/// <summary>
		/// Gets the controller context.
		/// </summary>
		protected IPresentContext Context => _context;

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		protected bool IsDismissed => _context.IsDismissed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">A controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IPresentContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_context.View.Command += OnCommand;
		}

		/// <summary>
		/// Dismissses this controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		protected void Dismiss()
		{
			_context.Dismiss();
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		protected void ThrowIfDismissed()
		{
			if (_context.IsDismissed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called to process a command.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		protected virtual bool OnCommand<TCommand>(TCommand command)
		{
			return false;
		}

		/// <summary>
		/// Called right before the controller becomes active. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDeactivate"/>
		protected virtual void OnActivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnActivate"/>
		protected virtual void OnDeactivate()
		{
		}

		/// <summary>
		/// Called when the controller has been presented. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		protected virtual void OnPresent()
		{
		}

		/// <summary>
		/// Called when the controller is being dismissed. Should not throw exceptions. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		/// <seealso cref="ThrowIfDismissed"/>
		protected virtual void OnDismiss()
		{
		}

		/// <summary>
		/// Called on each frame. Default implementation does nothing.
		/// </summary>
		protected virtual void OnUpdate(float frameTime)
		{
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets a view managed by the controller. Never returns <see langword="null"/>.
		/// </summary>
		public IView View => _context.View;

		#endregion

		#region IViewControllerEvents

		void IViewControllerEvents.OnActivate()
		{
			Debug.Assert(!IsDismissed);
			OnActivate();
		}

		void IViewControllerEvents.OnDeactivate()
		{
			Debug.Assert(!IsDismissed);
			OnDeactivate();
		}

		void IViewControllerEvents.OnPresent()
		{
			Debug.Assert(!IsDismissed);
			OnPresent();
		}

		void IViewControllerEvents.OnDismiss()
		{
			OnDismiss();
		}

		void IViewControllerEvents.OnUpdate(float frameTime)
		{
			Debug.Assert(!IsDismissed);
			OnUpdate(frameTime);
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
			if (command != null && !_context.IsDismissed)
			{
				return OnCommand(command);
			}

			return false;
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, EventArgs e)
		{
			Debug.Assert(sender == _context.View);

			if (e != null && !_context.IsDismissed)
			{
				OnCommand(e);
			}
		}

		#endregion
	}
}
