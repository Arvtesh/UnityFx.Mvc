﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <seealso cref="IViewController{TView}"/>
	/// <seealso cref="IView"/>
	public interface IViewController : IObjectId, IDismissable
	{
	}
}