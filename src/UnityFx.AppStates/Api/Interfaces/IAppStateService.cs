// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application state service.
	/// </summary>
	/// <seealso cref="AppState"/>
	public interface IAppStateService : IPresenter, IDisposable
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
		/// Gets the service settings.
		/// </summary>
		IAppStateServiceSettings Settings { get; }

		/// <summary>
		/// Gets the child states.
		/// </summary>
		IReadOnlyCollection<AppState> States { get; }

		/// <summary>
		/// Gets a value indicating whether thare are any pending operations.
		/// </summary>
		bool IsBusy { get; }
	}
}
