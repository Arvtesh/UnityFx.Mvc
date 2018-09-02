// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A scene based view.
	/// </summary>
	public class SceneView : AppView
	{
		#region data

		private Scene _scene;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="SceneView"/> class.
		/// </summary>
		public SceneView(string id, PresentOptions options)
			: base(id, options)
		{
		}

		#endregion

		#region AppView

		/// <inheritdoc/>
		protected override void SetVisible(bool visible)
		{
			if (_scene.IsValid() && _scene.isLoaded)
			{
				foreach (var go in _scene.GetRootGameObjects())
				{
					go.SetActive(visible);
				}
			}
		}

		/// <inheritdoc/>
		protected override void SetEnabled(bool enabled)
		{
			if (_scene.IsValid() && _scene.isLoaded)
			{
				foreach (var go in _scene.GetRootGameObjects())
				{
					foreach (var raycaster in go.GetComponentsInChildren<GraphicRaycaster>(true))
					{
						raycaster.enabled = enabled;
					}
				}
			}
		}

		/// <inheritdoc/>
		protected override IAsyncOperation LoadContent(string resourceId)
		{
			_scene = SceneManager.GetSceneByName(resourceId);
			return SceneManager.LoadSceneAsync(resourceId, LoadSceneMode.Additive).ToAsync();
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && _scene.IsValid() && _scene.isLoaded)
			{
				SceneManager.UnloadSceneAsync(_scene);
				_scene = default(Scene);
			}
		}

		#endregion

		#region IComponentContainer

		/// <inheritdoc/>
		public override TComponent GetComponent<TComponent>()
		{
			if (_scene.IsValid() && _scene.isLoaded)
			{
				foreach (var go in _scene.GetRootGameObjects())
				{
					var c = go.GetComponent<TComponent>();

					if (c != null)
					{
						return c;
					}
				}
			}

			return default(TComponent);
		}

		/// <inheritdoc/>
		public override TComponent GetComponentRecursive<TComponent>()
		{
			if (_scene.IsValid() && _scene.isLoaded)
			{
				foreach (var go in _scene.GetRootGameObjects())
				{
					var c = go.GetComponentInChildren<TComponent>(true);

					if (c != null)
					{
						return c;
					}
				}
			}

			return default(TComponent);
		}

		/// <inheritdoc/>
		public override TComponent[] GetComponents<TComponent>()
		{
			if (_scene.IsValid() && _scene.isLoaded)
			{
				var result = new List<TComponent>();

				foreach (var go in _scene.GetRootGameObjects())
				{
					go.GetComponents(result);
				}

				return result.ToArray();
			}

			return null;
		}

		/// <inheritdoc/>
		public override TComponent[] GetComponentsRecursive<TComponent>()
		{
			if (_scene.IsValid() && _scene.isLoaded)
			{
				var result = new List<TComponent>();

				foreach (var go in _scene.GetRootGameObjects())
				{
					go.GetComponentsInChildren(true, result);
				}

				return result.ToArray();
			}

			return null;
		}

		#endregion

		#region implementation
		#endregion
	}
}
