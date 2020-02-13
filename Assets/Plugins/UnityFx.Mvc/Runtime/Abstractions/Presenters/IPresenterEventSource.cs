// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provider of events for <see cref="IPresenter"/>.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresenterEvents"/>
	/// <seealso cref="IPresenterBuilder"/>
	public interface IPresenterEventSource
	{
		/// <summary>
		/// Adds a presenter events.
		/// </summary>
		void AddPresenter(IPresenterEvents presenter);

		/// <summary>
		/// Removes a presenter events.
		/// </summary>
		void RemovePresenter(IPresenterEvents presenter);
	}
}
