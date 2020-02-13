// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines events of <see cref="IPresenter"/>.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresenterEventSource"/>
	/// <seealso cref="IPresenterBuilder"/>
	public interface IPresenterEvents
	{
		/// <summary>
		/// Update event. Called on each frame.
		/// </summary>
		void Update();
	}
}
