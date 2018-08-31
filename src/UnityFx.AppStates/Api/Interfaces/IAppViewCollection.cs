// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.AppStates.Common;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A read-only collection of <see cref="IAppView"/>.
	/// </summary>
	/// <seealso cref="IAppView"/>
	public interface IAppViewCollection : ITreeListCollection<IAppView>
	{
	}
}
