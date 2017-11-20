// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A generic application view.
	/// </summary>
	/// <seealso cref="IAppViewFactory"/>
	public interface IAppView : ICollection<GameObject>, IDisposable
	{
		/// <summary>
		/// Returns name of the view. Read only.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Return user-specified data (if any). Read only.
		/// </summary>
		object UserData { get; }

		/// <summary>
		/// Returns view bounds (in world space) based on its content. Read only.
		/// </summary>
		Bounds Bounds { get; }

		/// <summary>
		/// Enabled or disapbes the view (enabled views are visible, disabled are not visible and not interactable).
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Enabled or disapbes input processing for the view.
		/// </summary>
		bool Interactable { get; set; }
	}
}