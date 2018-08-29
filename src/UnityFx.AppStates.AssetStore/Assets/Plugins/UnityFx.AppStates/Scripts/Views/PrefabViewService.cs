// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A view service for <see cref="PrefabView"/> views.
	/// </summary>
	public class PrefabViewService : AppViewService
	{
		#region data

		private readonly IPrefabLoader _prefabLoader;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PrefabViewService"/> class.
		/// </summary>
		public PrefabViewService(IPrefabLoader prefabLoader)
		{
			_prefabLoader = prefabLoader;
		}

		#endregion

		#region AppViewService

		/// <inheritdoc/>
		protected override IAppView CreateView(string id, PresentOptions options)
		{
			return new PrefabView(id, options, _prefabLoader);
		}

		#endregion

		#region implementation
		#endregion
	}
}
