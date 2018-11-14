// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <seealso cref="IView"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IViewController : ICommandTarget, IDisposable
	{
		/// <summary>
		/// Gets the controller name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="View"/> can be safely used.
		/// </summary>
		/// <seealso cref="View"/>
		bool IsViewLoaded { get; }

		/// <summary>
		/// Gets a view managed by the controller.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not available.</exception>
		/// <seealso cref="IsViewLoaded"/>
		IView View { get; }
	}
}
