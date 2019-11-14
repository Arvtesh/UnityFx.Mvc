// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Net;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Context data for an <see cref="IViewController"/> instance. The class is a link between <see cref="IPresenter"/> and its controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="IViewController"/> implementations.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IPresentContext : IViewControllerInfo, IPresenter, IServiceProvider
	{
		/// <summary>
		/// Gets the controller view.
		/// </summary>
		IView View { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Schedules a callback to be called in the specified <paramref name="timeout"/>.
		/// </summary>
		/// <param name="timerCallback">The callback to be called when the time is out.</param>
		/// <param name="timeout">Timeout value in seconds.</param>
		void Schedule(Action<float> timerCallback, float timeout);

		/// <summary>
		/// Dismisses the controller.
		/// </summary>
		/// <remarks>
		/// This call also dismisses all controllers presented by the owner.
		/// </remarks>
		/// <seealso cref="IsDismissed"/>
		void Dismiss();
	}
}
