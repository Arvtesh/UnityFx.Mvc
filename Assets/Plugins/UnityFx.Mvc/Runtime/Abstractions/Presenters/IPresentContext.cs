// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Context data for a <see cref="IViewController"/> instance. The class is a link between <see cref="IPresenter"/> and its controllers.
	/// </summary>
	/// <seealso cref="IPresentContext{TResult}"/>
	/// <seealso cref="IViewController"/>
	public interface IPresentContext : IPresenter, IServiceProvider
	{
		/// <summary>
		/// Gets unique identifier of the controller.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets the controller present arguments.
		/// </summary>
		/// <seealso cref="PresentOptions"/>
		PresentArgs PresentArgs { get; }

		/// <summary>
		/// Gets the present flags used when instantiating the controller.
		/// </summary>
		/// <seealso cref="PresentArgs"/>
		PresentOptions PresentOptions { get; }

		/// <summary>
		/// Gets time elapsed since the controller has been presented (in seconds).
		/// </summary>
		float PresentTime { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is active.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		bool IsActive { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="IsActive"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Gets the controller view.
		/// </summary>
		IView View { get; }

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
