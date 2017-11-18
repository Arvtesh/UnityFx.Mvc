// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A collection of <see cref="IAppView"/>.
	/// </summary>
	public interface IAppViewManager : IReadOnlyCollection<IAppView>
	{
		/// <summary>
		/// Creates an empty view with the specified name.
		/// </summary>
		IAppView AddView(string name, object userData = null);

		/// <summary>
		/// Removes the specified view from the manager.
		/// </summary>
		bool RemoveView(IAppView view);

		/// <summary>
		/// Removes all views.
		/// </summary>
		void Clear();
	}
}
