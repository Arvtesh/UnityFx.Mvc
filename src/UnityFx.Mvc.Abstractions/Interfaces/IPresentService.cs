// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic presenter service.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IViewController"/>
	public interface IPresentService : IPresenter, ICommandTarget, IDisposable
	{
		/// <summary>
		/// Raised when a new present operation is initiated.
		/// </summary>
		event EventHandler<PresentInitiatedEventArgs> PresentInitiated;

		/// <summary>
		/// Raised when a present operation is completed (either successfully or not).
		/// </summary>
		event EventHandler<PresentCompletedEventArgs> PresentCompleted;

		/// <summary>
		/// Raised when a new dismiss operation is initiated.
		/// </summary>
		event EventHandler<DismissInitiatedEventArgs> DismissInitiated;

		/// <summary>
		/// Raised when a dismiss operation is completed (either successfully or not).
		/// </summary>
		event EventHandler<DismissCompletedEventArgs> DismissCompleted;

		/// <summary>
		/// Gets service provider used to resolve controller dependencies.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets an active <see cref="IViewController"/> (or <see langword="null"/>).
		/// </summary>
		IViewController ActiveController { get; }

		/// <summary>
		/// Gets a value indicating whether thare are any pending operations.
		/// </summary>
		bool IsBusy { get; }
	}
}
