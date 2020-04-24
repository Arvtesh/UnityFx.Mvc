// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extension methods for <see cref="IViewFactoryBuilder"/>.
	/// </summary>
	/// <seealso cref="IView"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IViewFactoryBuilderExtensions
	{
		/// <summary>
		/// Adds preloaded view prefabs.
		/// </summary>
		/// <param name="prefabs">The preloaded prefabs.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="prefabs"/> is <see langword="null"/>.</exception>
		public static IViewFactoryBuilder AddViewPrefabs(this IViewFactoryBuilder builder, params GameObject[] prefabs)
		{
			if (prefabs is null)
			{
				throw new ArgumentNullException(nameof(prefabs));
			}

			foreach (var go in prefabs)
			{
				if (go)
				{
					builder.AddViewPrefab(go.name, go);
				}
			}

			return builder;
		}

		/// <summary>
		/// Adds layers.
		/// </summary>
		/// <param name="layers">The layers array.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="layers"/> is <see langword="null"/>.</exception>
		public static IViewFactoryBuilder AddLayers(this IViewFactoryBuilder builder, params Transform[] layers)
		{
			if (layers is null)
			{
				throw new ArgumentNullException(nameof(layers));
			}

			foreach (var layer in layers)
			{
				if (layer)
				{
					builder.AddLayer(layer);
				}
			}

			return builder;
		}
	}
}
