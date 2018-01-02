// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A collection of <see cref="IAppView"/> instances.
	/// </summary>
	/// <seealso cref="IAppView"/>
	/// <seealso cref="IAppViewService"/>
	public interface IAppViewLayer : IAppViewFactory, IEnumerable<IAppView>, IDisposable
	{
		/// <summary>
		/// Returns a collection of views managed by the manager. Read only.
		/// </summary>
		IReadOnlyList<IAppView> Views { get; }
	}
}
