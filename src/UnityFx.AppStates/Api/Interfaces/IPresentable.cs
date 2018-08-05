// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable object.
	/// </summary>
	public interface IPresentable : IDismissable
	{
		/// <summary>
		/// Gets the state identifier.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets a value indicating whether the instance is active.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets the atached view instance.
		/// </summary>
		IAppView View { get; }
	}
}
