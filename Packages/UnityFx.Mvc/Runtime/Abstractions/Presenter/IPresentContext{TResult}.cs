// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Context data for an <see cref="IViewController"/> instance. The class is a link between <see cref="IPresenter"/> and its controllers.
	/// </summary>
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IViewController"/>
	public interface IPresentContext<TResult> : IPresentContext
	{
		/// <summary>
		/// Dismisses the controller with a specific <paramref name="result"/>.
		/// </summary>
		void Dismiss(TResult result);
	}
}
