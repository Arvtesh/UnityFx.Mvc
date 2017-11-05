// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A view implementation used by <see cref="IAppStateView"/>.
	/// </summary>
	/// <seealso cref="IAppStateView"/>
	public interface IAppStateViewProxy : IEnumerable<GameObject>, IDisposable
	{
		/// <summary>
		/// Gets or sets view z-order priority. Greater values place view closer to viewer.
		/// </summary>
		int Priority { get; set; }
	}
}