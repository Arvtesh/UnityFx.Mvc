// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Context data for an <see cref="IViewController"/> instance. The class is a link between <see cref="IPresenter"/> and its controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="IViewController"/> implementations.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerContext : IPresenter, IServiceProvider
	{
		/// <summary>
		/// Gets an unique controller id.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets the controller name.
		/// </summary>
		string ControllerTypeName { get; }

		/// <summary>
		/// Gets the view name.
		/// </summary>
		string ViewTypeName { get; }

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
		void Dismiss();
	}
}
