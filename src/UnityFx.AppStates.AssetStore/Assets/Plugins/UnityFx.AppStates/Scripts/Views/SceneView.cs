// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
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
		private ISite _site;
		private bool _visible;
		private bool _enabled;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the object is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected bool IsDisposed
		{
			get
			{
				return _disposed;
			}
		}

		/// <summary>
		/// Gets the component's container.
		/// </summary>
		protected IContainer Container
		{
			get
			{
				return _site != null ? _site.Container : null;
			}
		}

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

			SceneManager.sceneUnloaded += OnSceneUnloaded;

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
		/// Called when <see cref="Visible"/> property value changes.
		/// </summary>
		/// <param name="visible">The new value of <see cref="Visible"/> propoerty.</param>
		/// <seealso cref="OnEnabledChanged(bool)"/>
		protected virtual void OnVisibleChanged(bool visible)
		{
			foreach (var go in _scene.GetRootGameObjects())
			{
				go.SetActive(visible);
			}

			if (VisibleChanged != null)
			{
				VisibleChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Called when <see cref="Enabled"/> property value changes.
		/// </summary>
		/// <param name="enabled">The new value of <see cref="Enabled"/> propoerty.</param>
		/// <seealso cref="OnVisibleChanged(bool)"/>
		protected virtual void OnEnabledChanged(bool enabled)
		{
			foreach (var go in _scene.GetRootGameObjects())
			{
				foreach (var c in go.GetComponentsInChildren<GraphicRaycaster>(true))
				{
					c.enabled = enabled;
				}
			}

			if (EnabledChanged != null)
			{
				EnabledChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Called when the object has been disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDisposed()
		{
			if (_site != null && _site.Container != null)
			{
				_site.Container.Remove(this);
			}

			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="OnDisposed"/>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					SceneManager.sceneUnloaded -= OnSceneUnloaded;
					SceneManager.UnloadSceneAsync(_scene);
					OnDisposed();
				}
			}
		}

		#endregion

		#region IView

		/// <summary>
		/// Raised when the <see cref="Visible"/> property value changes.
		/// </summary>
		/// <seealso cref="EnabledChanged"/>
		/// <seealso cref="OnVisibleChanged(bool)"/>
		/// <seealso cref="Visible"/>
		public event EventHandler VisibleChanged;

		/// <summary>
		/// Raised when the <see cref="Enabled"/> property value changes.
		/// </summary>
		/// <seealso cref="VisibleChanged"/>
		/// <seealso cref="OnEnabledChanged(bool)"/>
		/// <seealso cref="Enabled"/>
		public event EventHandler EnabledChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		/// <seealso cref="Enabled"/>
		/// <seealso cref="OnVisibleChanged(bool)"/>
		/// <seealso cref="VisibleChanged"/>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				ThrowIfDisposed();

				if (_visible != value)
				{
					_visible = value;
					OnVisibleChanged(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view can respond to user interaction.
		/// </summary>
		/// <seealso cref="Visible"/>
		/// <seealso cref="OnEnabledChanged(bool)"/>
		/// <seealso cref="EnabledChanged"/>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				ThrowIfDisposed();

				if (_enabled != value)
				{
					_enabled = value;
					OnEnabledChanged(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets an arbitrary object value that can be used to store custom information about this object.
		/// </summary>
		public object Tag { get; set; }

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

		#region IComponent

		/// <summary>
		/// Gets or sets the <see cref="ISite"/> associated with the <see cref="IComponent"/>.
		/// </summary>
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		/// <summary>
		/// Represents the method that handles the dispose event of a component.
		/// </summary>
		public event EventHandler Disposed;

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="OnDisposed"/>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private void OnSceneUnloaded(Scene scene)
		{
			if (!_disposed && _scene == scene)
			{
				_disposed = true;
				SceneManager.sceneUnloaded -= OnSceneUnloaded;
				OnDisposed();
			}
		}

		#endregion
	}
}
