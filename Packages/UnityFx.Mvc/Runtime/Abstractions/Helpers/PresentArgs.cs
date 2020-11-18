// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Runtime arguments for <see cref="IPresenter"/> methods.
	/// </summary>
	public class PresentArgs : IPresentArgs
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Gets or sets a user-defined  data.
		/// </summary>
		public object UserData { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		public PresentArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		public PresentArgs(object userData)
		{
			UserData = userData;
		}

		#endregion

		#region IPresentArgs

		/// <inheritdoc/>
		public PresentOptions PresentOptions { get; set; }

		/// <inheritdoc/>
		public Transform Transform { get; set; }

		#endregion

		#region implementation
		#endregion
	}
}
