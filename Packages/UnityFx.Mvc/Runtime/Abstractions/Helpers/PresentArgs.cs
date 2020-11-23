// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Runtime arguments for <see cref="IPresenter"/> methods.
	/// </summary>
	public class PresentArgs
	{
		/// <summary>
		/// Gets or sets peprent transform.
		/// </summary>
		public Transform Transform { get; set; }

		/// <summary>
		/// Gets or sets runtime present options.
		/// </summary>
		public PresentOptions PresentOptions { get; set; }

		/// <summary>
		/// Gets or sets a user-defined  data.
		/// </summary>
		public object UserData { get; set; }
	}
}
