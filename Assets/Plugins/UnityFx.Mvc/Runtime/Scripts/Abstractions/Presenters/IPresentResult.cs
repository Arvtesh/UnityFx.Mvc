// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresenter"/>
	public interface IPresentResult : IDismissable
	{
		/// <summary>
		/// Raised right after the controller has been presented.
		/// </summary>
		/// <seealso cref="IsPresented"/>
		event EventHandler<PresentCompletedEventArgs> Presented;

		/// <summary>
		/// Gets a value indicating whether the <see cref="Controller"/> is presented.
		/// </summary>
		/// <seealso cref="PresentCompleted"/>
		/// <seealso cref="Controller"/>
		bool IsPresented { get; }

		/// <summary>
		/// Gets the view controller.
		/// </summary>
		/// <seealso cref="PresentCompleted"/>
		/// <seealso cref="Dismissed"/>
		IViewController Controller { get; }

		/// <summary>
		/// Gets the view.
		/// </summary>
		IView View { get; }
	}
}
