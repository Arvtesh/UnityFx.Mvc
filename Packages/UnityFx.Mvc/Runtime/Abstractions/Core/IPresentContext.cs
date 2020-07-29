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
	public interface IPresentContext : IServiceProvider
	{
		/// <summary>
		/// Gets time elapsed since the controller has been created (in seconds).
		/// </summary>
		float PresentTime { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is active.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss()"/>
		/// <seealso cref="Dismiss(Exception)"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Gets the controller view.
		/// </summary>
		IView View { get; }

		/// <summary>
		/// Gets the controller tag. The meaning of this field is defined by user. Typically used to group controllers.
		/// </summary>
		int Tag { get; }

		/// <summary>
		/// Schedules a callback to be called in the specified <paramref name="timeout"/>.
		/// </summary>
		/// <param name="timerCallback">The callback to be called when the time is out.</param>
		/// <param name="timeout">Timeout value in seconds.</param>
		void Schedule(Action<float> timerCallback, float timeout);

		/// <summary>
		/// Dismisses the controller with exception.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		/// <seealso cref="Dismiss()"/>
		void Dismiss(Exception e);

		/// <summary>
		/// Dismisses the controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		/// <seealso cref="Dismiss(Exception)"/>
		void Dismiss();
	}
}
