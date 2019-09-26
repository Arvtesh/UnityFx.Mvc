// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresenter"/>
	public interface IPresentResult
	{
		/// <summary>
		/// Raised when the <see cref="Controller"/> is presented.
		/// </summary>
		/// <seealso cref="IsPresented"/>
		/// <seealso cref="Controller"/>
		event EventHandler<PresentCompletedEventArgs> PresentCompleted;

		/// <summary>
		/// Raised when the <see cref="Controller"/> is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="IsDismissed"/>
		/// <seealso cref="Controller"/>
		event EventHandler Dismissed;

		/// <summary>
		/// Gets a value indicating whether the <see cref="Controller"/> is presented.
		/// </summary>
		/// <seealso cref="PresentCompleted"/>
		/// <seealso cref="Controller"/>
		bool IsPresented { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="Controller"/> is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="Dismissed"/>
		/// <seealso cref="Controller"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Gets the present task.
		/// </summary>
		Task<IViewController> PresentTask { get; }

		/// <summary>
		/// Gets the dismiss task.
		/// </summary>
		Task DismissTask { get; }

		/// <summary>
		/// Gets the view controller.
		/// </summary>
		/// <seealso cref="PresentCompleted"/>
		/// <seealso cref="Dismissed"/>
		IViewController Controller { get; }

		/// <summary>
		/// Dismisses the <see cref="Controller"/>.
		/// </summary>
		/// <seealso cref="Dismissed"/>
		/// <seealso cref="IsDismissed"/>
		void Dismiss();
	}
}
