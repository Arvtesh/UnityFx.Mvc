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
	public static class UGUIViewServiceBuilderExtensions
	{
		/// <summary>
		/// Applies the specififed configuration.
		/// </summary>
		/// <param name="config">The configuration asset.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="config"/> is <see langword="null"/>.</exception>
		public static ViewServiceBuilder UseConfig(this ViewServiceBuilder builder, UGUIMvcConfig config)
		{
			return builder.UseConfig((MvcConfig)config);
		}
	}
}
