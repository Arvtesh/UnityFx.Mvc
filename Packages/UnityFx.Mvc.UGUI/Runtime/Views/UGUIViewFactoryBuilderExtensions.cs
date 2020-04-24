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
	public static class UGUIViewFactoryBuilderExtensions
	{
		/// <summary>
		/// Applies the specififed configuration.
		/// </summary>
		/// <param name="config">The configuration asset.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="config"/> is <see langword="null"/>.</exception>
		public static IViewFactoryBuilder UseConfig(this IViewFactoryBuilder builder, UGUIMvcConfig config)
		{
			if (config is null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			foreach (var item in config.Prefabs)
			{
				builder.AddViewPrefab(item.Key, item.Value);
			}

			return builder.UsePopupBackgoundColor(config.PopupBackgroundColor);
		}
	}
}
