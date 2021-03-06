﻿// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation. Disposing the present result dismisses the attached controller (and its view).
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresenter"/>
	public interface IPresentResult : IPresentInfo, ICommandTarget, IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		bool IsDismissed { get; }

		/// <summary>
		/// Gets a <see cref="System.Threading.Tasks.Task"/> instance that can be used to await the operation completion (i.e. until the controller is dismissed).
		/// </summary>
		Task Task { get; }
	}
}
