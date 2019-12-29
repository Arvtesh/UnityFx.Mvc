// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation. Can be used very much like <see cref="Task"/>.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerAccess<out TController> where TController : IViewController
	{
		/// <summary>
		/// Gets the view controller.
		/// </summary>
		/// <seealso cref="View"/>
		TController Controller { get; }

		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <seealso cref="Controller"/>
		IView View { get; }
	}
}
