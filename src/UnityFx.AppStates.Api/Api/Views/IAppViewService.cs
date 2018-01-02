// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A collection of view layers.
	/// </summary>
	/// <seealso cref="IAppViewLayer"/>
	/// <seealso cref="IAppView"/>
	public interface IAppViewService : IDisposable
	{
		/// <summary>
		/// Returns a collection of view layers managed by the service. Read only.
		/// </summary>
		IReadOnlyList<IAppViewLayer> Layers { get; }

		/// <summary>
		/// Adds a new layer on top of existing ones.
		/// </summary>
		IAppViewLayer AddLayer();

		/// <summary>
		/// Adds a new layer with the specified index. If a layers with the specified index exists
		/// the new layer is inserved right after the existing one.
		/// </summary>
		IAppViewLayer AddLayer(int index);
	}
}
