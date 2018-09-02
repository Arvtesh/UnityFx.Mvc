// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
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
		private readonly Transform _rootTransform;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PrefabViewService"/> class.
		/// </summary>
		public PrefabViewService(IPrefabLoader prefabLoader, Transform rootTransform)
		{
			if (prefabLoader == null)
			{
				throw new ArgumentNullException("prefabLoader");
			}

			if (rootTransform == null)
			{
				throw new ArgumentNullException("rootTransform");
			}

			_prefabLoader = prefabLoader;
			_rootTransform = rootTransform;
		}

		#endregion

		#region AppViewService

		/// <inheritdoc/>
		protected override IAppView CreateView(string id, PresentOptions options)
		{
			return new PrefabView(id, options, _prefabLoader, _rootTransform);
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// TODO
			}

			base.Dispose(disposing);
		}

		#endregion

		#region implementation
		#endregion
	}
}
