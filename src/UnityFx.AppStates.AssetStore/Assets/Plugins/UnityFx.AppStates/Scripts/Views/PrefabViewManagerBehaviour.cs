// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Prefab view manager.
	/// </summary>
	[RequireComponent(typeof(PrefabViewLoaderBehaviour))]
	public sealed class PrefabViewManagerBehaviour : ViewManagerBehaviour<PrefabViewBehaviour>
	{
	}
}
