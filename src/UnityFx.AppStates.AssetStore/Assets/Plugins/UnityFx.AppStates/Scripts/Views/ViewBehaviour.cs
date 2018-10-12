// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A <see cref="MonoBehaviour"/>-based view.
	/// </summary>
	public class ViewBehaviour : ComponentBehaviour, IView
	{
		#region data

		private bool _visible;
		private bool _enabled;

		#endregion

		#region interface

		/// <summary>
		/// Called when <see cref="Visible"/> property value changes.
		/// </summary>
		/// <param name="visible">The new value of <see cref="Visible"/> propoerty.</param>
		/// <seealso cref="OnEnabledChanged(bool)"/>
		protected virtual void OnVisibleChanged(bool visible)
		{
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
			foreach (var c in GetComponentsInChildren<GraphicRaycaster>(true))
			{
				c.enabled = enabled;
			}

			if (EnabledChanged != null)
			{
				EnabledChanged(this, EventArgs.Empty);
			}
		}

		#endregion

		#region MonoBehaviour

		/// <summary>
		/// A <see cref="MonoBehaviour"/> enable handler.
		/// </summary>
		/// <seealso cref="OnDisable"/>
		protected virtual void OnEnable()
		{
			if (!_visible)
			{
				_visible = true;
				OnVisibleChanged(true);
			}
		}

		/// <summary>
		/// A <see cref="MonoBehaviour"/> enable handler.
		/// </summary>
		/// <seealso cref="OnEnable"/>
		protected virtual void OnDisable()
		{
			if (_visible)
			{
				_visible = false;
				OnVisibleChanged(false);
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
					gameObject.SetActive(value);
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
		public int Id
		{
			get
			{
				return GetInstanceID();
			}
		}

		/// <summary>
		/// Gets or sets the identifying name of the object.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				ThrowIfDisposed();
				name = value;
			}
		}

		#endregion
	}
}
