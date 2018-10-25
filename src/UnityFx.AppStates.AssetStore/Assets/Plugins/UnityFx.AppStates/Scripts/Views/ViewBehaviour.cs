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
		/// Raised when <see cref="Visible"/> value changes.
		/// </summary>
		public event EventHandler VisibleChanged;

		/// <summary>
		/// Raised when <see cref="Enabled"/> value changes.
		/// </summary>
		public event EventHandler EnabledChanged;

		/// <summary>
		/// Raises <see cref="Command"/> event.
		/// </summary>
		/// <param name="commandId">Name of the property.</param>
		/// <seealso cref="Command"/>
		protected void NotifyCommand(string commandId)
		{
			if (Command != null)
			{
				Command(this, new CommandEventArgs(commandId));
			}
		}

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
		/// Raised when a user issues a command.
		/// </summary>
		public event EventHandler<CommandEventArgs> Command;

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
	}
}
