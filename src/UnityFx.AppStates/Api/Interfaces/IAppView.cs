// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view.
	/// </summary>
	public interface IAppView : ITreeListNode<IAppView>, IDisposable
	{
		/// <summary>
		/// Gets the view identifier.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets a value indicating whether the view is loaded or not.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Gets a value indicating whether the view is currently being loaded.
		/// </summary>
		bool IsLoading { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Initiates loading content of the view.
		/// </summary>
		IAsyncOperation Load();
	}
}
