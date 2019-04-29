// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

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
		/// <summary>
		/// Raised when the instance is presented.
		/// </summary>
		/// <seealso cref="IsPresented"/>
		event EventHandler<AsyncCompletedEventArgs> Presented;

		/// <summary>
		/// Gets a value indicating whether the controller is presented.
		/// </summary>
		/// <seealso cref="Presented"/>
		bool IsPresented { get; }
	}
}
