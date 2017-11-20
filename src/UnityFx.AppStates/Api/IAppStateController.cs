// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A user-defined application state controller.
	/// </summary>
	/// <remarks>
	/// An <see cref="IAppStateController"/> implementation can extend <see cref="IAppStateEvents"/> interface to receive state
	/// lifetime-related events. Inheriting <see cref="MonoBehaviour"/> is not required. If <see cref="IDisposable"/>
	/// is implemented the <see cref="IDisposable.Dispose"/> is called on the state instance right after popping it from
	/// the states stack.
	/// </remarks>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IAppStateContext"/>
	public interface IAppStateController
	{
	}
}