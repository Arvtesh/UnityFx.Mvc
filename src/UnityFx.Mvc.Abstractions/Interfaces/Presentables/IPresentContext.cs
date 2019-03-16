// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Context data for an <see cref="IPresentable"/> instance. The class is a link between <see cref="IPresenter"/> and its controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="IPresentable"/> implementations.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IPresentContext : IPresenter, IServiceProvider
	{
		/// <summary>
		/// Gets an unique controller id.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets a value indicating whether the controller has been presented (but not dismissed).
		/// </summary>
		bool IsPresented { get; }

		/// <summary>
		/// Gets a value indicating whether the controller has been dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is active.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is modal.
		/// </summary>
		bool IsModal { get; }

		/// <summary>
		/// Dismisses the controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		void Dismiss();
	}
}
