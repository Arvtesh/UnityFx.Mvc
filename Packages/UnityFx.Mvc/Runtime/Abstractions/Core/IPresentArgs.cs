// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Runtime arguments for <see cref="IPresenter"/> methods.
	/// </summary>
	public interface IPresentArgs
	{
		/// <summary>
		/// Gets present options.
		/// </summary>
		PresentOptions PresentOptions { get; }

		/// <summary>
		/// Gets a transform to attach view to (or <see langword="null"/>).
		/// </summary>
		Transform Transform { get; }
	}
}
