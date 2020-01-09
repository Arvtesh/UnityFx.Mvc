// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A view controller information.
	/// </summary>
	/// <remarks>
	/// As the name states, main responsibility of a view controller is managing its view.
	/// Controllers are created via a <see cref="IViewControllerFactory"/> instance.
	/// </remarks>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerInfo
	{
		/// <summary>
		/// Gets unique identifier of the controller.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets the deepling identifier for this controller.
		/// </summary>
		string DeeplinkId { get; }

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
	}
}
