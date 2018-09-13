// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	public interface IViewController : IObjectId, IDismissable
	{
	}
}
