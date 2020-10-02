// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAsyncPresentable
	{
		/// <summary>
		/// Initiates an asynchronous PRESENT operation.
		/// </summary>
		Task PresentAsync();
	}
}
