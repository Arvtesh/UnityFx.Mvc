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
	public interface IPresentContext : IPresenter, IServiceProvider
	{
		/// <summary>
		/// Gets the controller arguments.
		/// </summary>
		PresentArgs Args { get; }

		/// <summary>
		/// Gets the controller view.
		/// </summary>
		IView View { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is active.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		bool IsDismissed { get; }

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
