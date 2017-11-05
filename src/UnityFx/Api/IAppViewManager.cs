// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A controller for the <see cref="IAppStateView"/> hierarchy.
	/// </summary>
	internal interface IAppViewManager
	{
		/// <summary>
		/// Loads a view for the specified controller.
		/// </summary>
		Task<IAppStateViewProxy> LoadViewAsync(object viewController);
	}
}
