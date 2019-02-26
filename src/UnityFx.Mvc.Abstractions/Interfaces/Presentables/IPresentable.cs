// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A presentable view controller.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IPresentService"/>
	public interface IPresentable : IViewController, IDismissable
	{
	}
}
