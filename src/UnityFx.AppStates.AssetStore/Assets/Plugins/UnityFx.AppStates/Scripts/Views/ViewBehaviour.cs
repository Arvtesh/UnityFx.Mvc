// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
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
		/// Raises <see cref="PropertyChanged"/> event for a property with name <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <seealso cref="PropertyChanged"/>
		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Called when <see cref="Visible"/> property value changes.
		/// </summary>
		/// <param name="visible">The new value of <see cref="Visible"/> propoerty.</param>
		/// <seealso cref="OnEnabledChanged(bool)"/>
		protected virtual void OnVisibleChanged(bool visible)
		{
			NotifyPropertyChanged(nameof(Visible));
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

			NotifyPropertyChanged(nameof(Enabled));
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
		/// Gets the view name.
		/// </summary>
		public string Name
		{
			get
			{
				return GetName();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		/// <seealso cref="Enabled"/>
		/// <seealso cref="OnVisibleChanged(bool)"/>
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
					gameObject.SetActive(value);
					OnVisibleChanged(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view can respond to user interaction.
		/// </summary>
		/// <seealso cref="Visible"/>
		/// <seealso cref="OnEnabledChanged(bool)"/>
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

		#endregion

		#region INotifyPropertyChanged

		/// <summary>
		/// Raised when a property value changes.
		/// </summary>
		/// <seealso cref="NotifyPropertyChanged(string)"/>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
