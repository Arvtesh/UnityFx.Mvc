// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A target for PRESENT/DISMISS async notifications.
	/// </summary>
	/// <seealso cref="IPresentTarget"/>
	/// <seealso cref="IActivateTarget"/>
	/// <seealso cref="IViewController"/>
	public interface IAstncPresentTarget
	{
		/// <summary>
		/// Called when the object is presented.
		/// </summary>
		/// <seealso cref="DismissAsync"/>
		Task PresentAsync();

		/// <summary>
		/// Called when the object is dismissed.
		/// </summary>
		/// <seealso cref="PresentAsync"/>
		Task DismissAsync();
	}
}
