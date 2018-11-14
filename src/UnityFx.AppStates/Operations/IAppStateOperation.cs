// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Helper interface for managing sync/async operatino completion.
	/// </summary>
	internal interface IAppStateOperation
	{
		/// <summary>
		/// Sets asynchronous completion flag.
		/// </summary>
		void SetCompletedAsynchronously();
	}
}
