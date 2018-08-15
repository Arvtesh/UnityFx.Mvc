// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A read-only collection of <see cref="IAppView"/>.
	/// </summary>
	/// <seealso cref="IAppView"/>
#if NET35
	public interface IAppViewCollection : ICollection<IAppView>
#else
	public interface IAppViewCollection : IReadOnlyCollection<IAppView>
#endif
	{
	}
}
