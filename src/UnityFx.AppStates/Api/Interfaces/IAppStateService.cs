// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application state service.
	/// </summary>
	/// <seealso cref="IAppState"/>
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
		/// Gets or sets trace switch used by the <see cref="TraceSource"/> instance.
		/// </summary>
		SourceSwitch TraceSwitch { get; set; }

		/// <summary>
		/// Gets a collection of <see cref="TraceListener"/> instances attached to the <see cref="TraceSource"/> used for logging.
		/// </summary>
		TraceListenerCollection TraceListeners { get; }

		/// <summary>
		/// Gets a value indicating whether thare are any pending operations.
		/// </summary>
		bool IsBusy { get; }

		/// <summary>
		/// Gets the child states.
		/// </summary>
		IAppStateCollection States { get; }

		/// <summary>
		/// Gets active state (or <see langword="null"/>).
		/// </summary>
		IAppState ActiveState { get; }

		/// <summary>
		/// Gets the service settings.
		/// </summary>
		IAppStateServiceSettings Settings { get; }
	}
}
