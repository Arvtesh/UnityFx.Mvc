// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Net;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Context data for an <see cref="IViewController"/> instance. The class is a link between <see cref="IPresenter"/> and its controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="IViewController"/> implementations.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IPresentContext<TResult> : IPresentContext
	{
		/// <summary>
		/// Dismisses the controller.
		/// </summary>
		void Dismiss(TResult result);
	}
}
