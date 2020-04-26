// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A view manager.
	/// </summary>
	/// <seealso cref="IView"/>
	public interface IViewService : IViewFactory, IDisposable
	{
		/// <summary>
		/// Gets popup background color.
		/// </summary>
		Color PopupBackgroundColor { get; }

		/// <summary>
		/// Gets a read-only collection of view layers.
		/// </summary>
		IReadOnlyList<Transform> Layers { get; }

		/// <summary>
		/// Gets a read-only collection of presented views.
		/// </summary>
		IReadOnlyCollection<IView> Views { get; }

		/// <summary>
		/// Gets a read-only collection of loaded view prefabs.
		/// </summary>
		IReadOnlyDictionary<string, GameObject> Prefabs { get; }
	}
}
