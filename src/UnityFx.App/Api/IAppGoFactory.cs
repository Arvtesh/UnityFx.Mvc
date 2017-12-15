// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A factory for <see cref="GameObject"/> instances.
	/// </summary>
	public interface IAppGoFactory
	{
		/// <summary>
		/// Creates a <see cref="GameObject"/> instance with the specified name.
		/// </summary>
		GameObject CreateGameObject(string name);
	}
}
