// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Read-only view controller info.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerInfo
	{
		/// <summary>
		/// Gets unique identifier of the controller.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets the controller present arguments.
		/// </summary>
		PresentArgs PresentArgs { get; }

		/// <summary>
		/// Gets the present flags used when instantiating the controller.
		/// </summary>
		PresentOptions PresentOptions { get; }

		/// <summary>
		/// Gets time elapsed since the controller has been presented (in seconds).
		/// </summary>
		float PresentTime { get; }

		/// <summary>
		/// Gets a value indicating whether the controller is active.
		/// </summary>
		bool IsActive { get; }
	}
}
