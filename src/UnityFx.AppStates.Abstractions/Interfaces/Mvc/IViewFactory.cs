// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory of <see cref="IView"/> instances.
	/// </summary>
	public interface IViewFactory
	{
		/// <summary>
		/// Asynchronously loads the specified view.
		/// </summary>
		IAsyncOperation<IView> LoadViewAsync(string name, string resourceId, ViewOptions options, IView insertAfter);
	}
}
