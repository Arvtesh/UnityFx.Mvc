﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable object.
	/// </summary>
	public interface IPresentable : IDismissable
	{
		/// <summary>
		/// Gets a value indicating whether the state is active (i.e. can process user input).
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets the atached view instance.
		/// </summary>
		IAppView View { get; }
	}
}
