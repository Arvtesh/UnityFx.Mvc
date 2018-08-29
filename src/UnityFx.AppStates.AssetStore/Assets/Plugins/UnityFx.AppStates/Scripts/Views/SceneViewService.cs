// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A view service for <see cref="SceneView"/> views.
	/// </summary>
	public class SceneViewService : AppViewService
	{
		#region data
		#endregion

		#region AppViewService

		/// <inheritdoc/>
		protected override IAppView CreateView(string id, PresentOptions options)
		{
			return new SceneView(id, options);
		}

		#endregion

		#region implementation
		#endregion
	}
}
