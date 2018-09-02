// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable object.
	/// </summary>
	public interface IPresentable<TView> : IPresentable where TView : class
	{
		/// <summary>
		/// Gets the <typeparamref name="TView"/> view component.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not loaded or the <typeparamref name="TView"/> component is not attached to the view.</exception>
		TView ViewAspect { get; }
	}
}
