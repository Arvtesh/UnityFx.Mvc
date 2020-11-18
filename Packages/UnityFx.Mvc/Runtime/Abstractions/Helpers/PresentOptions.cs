// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Enumerates view controller present options.
	/// </summary>
	[Flags]
	public enum PresentOptions
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,

		/// <summary>
		/// If set the caller presenter is dismissed.
		/// </summary>
		DismissCurrent = 1,

		/// <summary>
		/// If set all controllers are dismissed before presenting the new one.
		/// </summary>
		DismissAll = 2
	}
}
