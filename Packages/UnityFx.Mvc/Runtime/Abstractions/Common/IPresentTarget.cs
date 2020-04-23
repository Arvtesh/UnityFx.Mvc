// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A target for PRESENT/DISMISS notifications.
	/// </summary>
	/// <seealso cref="IActivateTarget"/>
	/// <seealso cref="IViewController"/>
	public interface IPresentTarget
	{
		/// <summary>
		/// Called when the object is presented.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		void Present();

		/// <summary>
		/// Called when the object is dismissed.
		/// </summary>
		/// <seealso cref="Present"/>
		void Dismiss();
	}
}
