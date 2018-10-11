// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of a <see cref="Scene"/>-based view.
	/// </summary>
	public class SceneView : IView
	{
		#region data

		private Scene _scene;
		private GameObject _go;
		private IView _goView;
		private bool _visible;
		private bool _enabled;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="SceneView"/> class.
		/// </summary>
		/// <param name="scene">The scene to wrap.</param>
		public SceneView(Scene scene)
		{
			if (!scene.IsValid() || !scene.isLoaded)
			{
				throw new ArgumentException("Invalid scene. The scene is expected to be loaded.", "scene");
			}

			_scene = scene;
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the object is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="visible"></param>
		protected virtual void SetVisible(bool visible)
		{
			foreach (var go in _scene.GetRootGameObjects())
			{
				go.SetActive(visible);
			}
		}

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="enabled"></param>
		protected virtual void SetEnabled(bool enabled)
		{
			foreach (var go in _scene.GetRootGameObjects())
			{
				foreach (var c in go.GetComponentsInChildren<GraphicRaycaster>(true))
				{
					c.enabled = enabled;
				}
			}
		}

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					SceneManager.UnloadSceneAsync(_scene);
				}
			}
		}

		#endregion

		#region IView

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				ThrowIfDisposed();
				SetVisible(value);
				_visible = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view is enabled (i.e. accepts user input).
		/// </summary>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				ThrowIfDisposed();
				SetEnabled(value);
				_enabled = value;
			}
		}

		#endregion

		#region IObjectId

		/// <summary>
		/// Gets the instance identifier.
		/// </summary>
		public string Id
		{
			get
			{
				return GetType().Name;
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <seealso cref="Dispose(bool)"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
