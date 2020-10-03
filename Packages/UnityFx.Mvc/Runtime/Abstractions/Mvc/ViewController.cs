// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Default implementation of <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class ViewController : IViewController, IActivateEvents, IPresentEvents, IUpdatable, ICommandTarget
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
		/// Gets the presenter.
		/// </summary>
		protected IPresenter Presenter => (IPresenter)_context.GetService(typeof(IPresenter));

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss()"/>
		/// <seealso cref="Dismiss(Exception)"/>
		protected bool IsDismissed => _context.IsDismissed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">A controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IPresentContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Dismissses this controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		/// <seealso cref="Dismiss(Exception)"/>
		/// <seealso cref="ThrowIfDismissed"/>
		protected void Dismiss()
		{
			_context.Dismiss();
		}

		/// <summary>
		/// Dismissses this controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		/// <seealso cref="Dismiss()"/>
		/// <seealso cref="ThrowIfDismissed"/>
		protected void Dismiss(Exception e)
		{
			_context.Dismiss(e);
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dismiss()"/>
		/// <seealso cref="Dismiss(Exception)"/>
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
		protected virtual bool OnCommand(Command command, Variant args)
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
		protected virtual void OnUpdate()
		{
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets a view managed by the controller. Never returns <see langword="null"/>.
		/// </summary>
		public IView View => _context.View;

		#endregion

		#region IActivateEvents

		void IActivateEvents.OnActivate()
		{
			Debug.Assert(!IsDismissed);
			OnActivate();
		}

		void IActivateEvents.OnDeactivate()
		{
			Debug.Assert(!IsDismissed);
			OnDeactivate();
		}

		#endregion

		#region IPresentEvents

		void IPresentEvents.OnPresent()
		{
			Debug.Assert(!IsDismissed);
			OnPresent();
		}

		void IPresentEvents.OnDismiss()
		{
			OnDismiss();
		}

		#endregion

		#region IUpdatable

		void IUpdatable.Update()
		{
			Debug.Assert(!IsDismissed);
			OnUpdate();
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
			return OnCommand(command, args);
		}

		#endregion

		#region implementation
		#endregion
	}
}
