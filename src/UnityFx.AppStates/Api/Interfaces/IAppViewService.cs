// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory for <see cref="IAppView"/> instances.
	/// </summary>
	/// <seealso cref="IAppView"/>
	public interface IAppViewService : IDisposable
	{
		/// <summary>
		/// Gets child views.
		/// </summary>
		IAppViewCollection Views { get; }

		/// <summary>
		/// Creates an empty view with the specified <paramref name="id"/> on top of the <paramref name="insertAfter"/> one.
		/// If <paramref name="insertAfter"/> is <see langword="null"/> the view is created below all others.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		IAppView CreateView(string id, IAppView insertAfter, PresentOptions options);
	}
}
