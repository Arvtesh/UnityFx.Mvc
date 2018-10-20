// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A <see cref="Scene"/>-based view.
	/// </summary>
	public class SceneViewBehaviour : ViewBehaviour
	{
		#region data

		private Scene _scene;

		#endregion

		#region interface

		internal void Initialize(Scene scene)
		{
			if (!scene.IsValid() || !scene.isLoaded)
			{
				throw new ArgumentException("Invalid scene. The scene is expected to be loaded.", "scene");
			}

			SceneManager.sceneUnloaded += OnSceneUnloaded;

			_scene = scene;
		}

		#endregion

		#region DisposableBehaviour

		/// <summary>
		/// Called when the object has been disposed.
		/// </summary>
		protected override void OnDisposed()
		{
			SceneManager.sceneUnloaded -= OnSceneUnloaded;

			if (_scene.isLoaded)
			{
				SceneManager.UnloadSceneAsync(_scene);
			}

			base.OnDisposed();
		}

		#endregion

		#region implementation

		private void OnSceneUnloaded(Scene scene)
		{
			if (_scene == scene)
			{
				Dispose();
			}
		}

		#endregion
	}
}
