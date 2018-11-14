// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A <see cref="MonoBehaviour"/>-based view.
	/// </summary>
	public class ViewBehaviour : ComponentBehaviour, IView
	{
		#region data
		#endregion

		#region interface

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

		#endregion

		#region ComponentBehaviour

		/// <summary>
		/// Called when value of <see cref="IComponent.Site"/> changes.
		/// </summary>
		protected override void OnSiteChanged()
		{
			Command = null;
		}

		#endregion

		#region MonoBehaviour
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
		/// Gets or sets a value indicating whether the view is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				if (IsDisposed)
				{
					return false;
				}

				return gameObject.activeSelf;
			}
			set
			{
				ThrowIfDisposed();
				gameObject.SetActive(value);
			}
		}

		#endregion

		#region INotifyCommand

		/// <summary>
		/// Raised when a user issues a command.
		/// </summary>
		public event EventHandler<CommandEventArgs> Command;

		#endregion
	}
}
