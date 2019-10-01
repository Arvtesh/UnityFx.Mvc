﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation.
	/// </summary>
	/// <seealso cref="IPresentResult"/>
	public interface IPresentResult<TController> : IPresentResult where TController : IViewController
	{
		/// <summary>
		/// Gets the view controller.
		/// </summary>
		new TController Controller { get; }
	}
}
