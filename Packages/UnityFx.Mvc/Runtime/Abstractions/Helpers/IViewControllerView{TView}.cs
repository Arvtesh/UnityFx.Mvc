// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Tag interface for all <see cref="IViewController"/> implementations that have a specific view type assigned.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerView<TView>
	{
	}
}
