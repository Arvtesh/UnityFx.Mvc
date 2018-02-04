// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous operation with status information.
	/// </summary>
	public interface IAppStateOperation<T> : IAppStateOperation, IAsyncOperation<T>
	{
	}
}
