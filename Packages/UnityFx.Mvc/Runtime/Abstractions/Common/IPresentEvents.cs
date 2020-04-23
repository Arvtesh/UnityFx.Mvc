// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines event handlers for a <see cref="IViewController"/> implementation.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IPresentEvents
	{
		/// <summary>
		/// Called when the object is presented.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		void OnPresent();

		/// <summary>
		/// Called when the object is dismissed.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		void OnDismiss();
	}
}
