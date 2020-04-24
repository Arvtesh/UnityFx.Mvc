// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a class that provides the mechanisms to create and configure a view factory.
	/// </summary>
	/// <remarks>
	/// This interface defines fluent configuration API for <see cref="IViewFactory"/>. It is designed to be
	/// similar to ASP.NET <see href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.iapplicationbuilder"/>.
	/// </remarks>
	/// <seealso cref="IViewFactory"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Builder_pattern"/>
	public interface IViewFactoryBuilder
	{
		/// <summary>
		/// Adds a new view layer.
		/// </summary>
		/// <param name="transform">A <see cref="Transform"/> to use as a view root.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="transform"/> is <see langword="null"/>.</exception>
		/// <seealso cref="ViewControllerAttribute.Layer"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="Build"/>
		IViewFactoryBuilder AddLayer(Transform transform);

		/// <summary>
		/// Adds a preloaded view prefab.
		/// </summary>
		/// <param name="resourceId">A path to assosiate the <paramref name="prefab"/> with.</param>
		/// <param name="prefab">The preloaded prefab.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="resourceId"/> or <paramref name="prefab"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown is <paramref name="resourceId"/> is invalid.</exception>
		/// <seealso cref="Build"/>
		IViewFactoryBuilder AddViewPrefab(string resourceId, GameObject prefab);

		/// <summary>
		/// Applies the specififed configuration.
		/// </summary>
		/// <param name="config">The configuration asset.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="config"/> is <see langword="null"/>.</exception>
		/// <seealso cref="UsePopupBackgoundColor(Color)"/>
		/// <seealso cref="Build"/>
		IViewFactoryBuilder UseConfig(MvcConfig config);

		/// <summary>
		/// Sets a delegate to use to load prefabs.
		/// </summary>
		/// <param name="loadPrefabDelegate">A delegate to load a prefab for the specified path.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="loadPrefabDelegate"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the delegate is already set.</exception>
		/// <seealso cref="UsePopupBackgoundColor(Color)"/>
		/// <seealso cref="Build"/>
		IViewFactoryBuilder UseLoadDelegate(Func<string, Task<GameObject>> loadPrefabDelegate);

		/// <summary>
		/// Sets background color to use for popup views.
		/// </summary>
		/// <param name="backgroundColor">The popup background color.</param>
		/// <seealso cref="Build"/>
		IViewFactoryBuilder UsePopupBackgoundColor(Color backgroundColor);

		/// <summary>
		/// Builds a <see cref="IViewFactory"/> instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if presenter cannot be constructed (for instance, <see cref="IViewFactory"/> is not set and cannot be located).</exception>
		IViewFactory Build();
	}
}
