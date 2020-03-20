// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines present information shared between <see cref="IPresentResult"/> and <see cref="IPresentContext"/>.
	/// All data and methods are usable before the corresponding controller is created.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresentResult"/>
	/// <seealso cref="IPresentContext"/>
	public interface IPresentInfo
	{
		/// <summary>
		/// Gets unique identifier of the controller.
		/// </summary>
		/// <seealso cref="DeeplinkId"/>
		int Id { get; }

		/// <summary>
		/// Gets the deepling identifier for this controller.
		/// </summary>
		/// <seealso cref="Id"/>
		string DeeplinkId { get; }

		/// <summary>
		/// Gets the controller tag. The meaning of this field is defined by user. Typically used to group controllers.
		/// </summary>
		int Tag { get; }

		/// <summary>
		/// Gets type of the controller.
		/// </summary>
		Type ControllerType { get; }

		/// <summary>
		/// Gets the controller present arguments.
		/// </summary>
		/// <seealso cref="PresentOptions"/>
		PresentArgs PresentArgs { get; }

		/// <summary>
		/// Gets the present flags to use when the controller is created.
		/// </summary>
		/// <seealso cref="PresentArgs"/>
		PresentOptions PresentOptions { get; }
	}
}
