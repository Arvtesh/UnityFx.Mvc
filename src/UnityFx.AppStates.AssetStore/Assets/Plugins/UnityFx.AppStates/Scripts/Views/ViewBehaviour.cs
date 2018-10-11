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
	public class ViewBehaviour : DisposableMonoBehaviour, IView
	{
		#region data

		private bool _enabled;

		#endregion

		#region interface

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="visible"></param>
		protected virtual void OnSetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="enabled"></param>
		protected virtual void OnSetEnabled(bool enabled)
		{
			foreach (var c in GetComponentsInChildren<GraphicRaycaster>(true))
			{
				c.enabled = enabled;
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
				return gameObject.activeSelf;
			}
			set
			{
				ThrowIfDisposed();
				OnSetVisible(value);
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
				OnSetEnabled(value);
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
	}
}
