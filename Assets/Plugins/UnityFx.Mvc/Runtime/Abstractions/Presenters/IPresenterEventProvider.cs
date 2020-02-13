// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provider of vents for <see cref="IPresenter"/>.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresenterBuilder"/>
	public interface IPresenterEventProvider
	{
		/// <summary>
		/// Update event.
		/// </summary>
		event Action Update;
	}
}
