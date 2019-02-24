// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic presenter service.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IViewController"/>
	public interface IPresentService : IPresenter, ICommandTarget, IDisposable
	{
		/// <summary>
		/// Gets service provider used to resolve controller dependencies.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets an active <see cref="IViewController"/> (or <see langword="null"/>).
		/// </summary>
		IPresentable ActiveController { get; }

		/// <summary>
		/// Gets read-only stack of controllers maintained by the service.
		/// </summary>
		IPresentableStack Controllers { get; }
	}
}
