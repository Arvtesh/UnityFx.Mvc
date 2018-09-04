// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.AppStates.Samples
{
	/// <summary>
	/// About dialog view.
	/// </summary>
	/// <seealso cref="AboutController"/>
	public class AboutView : MonoBehaviour
	{
		#region data

		[SerializeField]
		private Button _closeButton = null;

		#endregion

		#region interface

		/// <summary>
		/// Raised when Close button is pressed.
		/// </summary>
		public event EventHandler ClosePressed;

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(OnClosePressed);
			}
		}

		#endregion

		#region implementation

		private void OnClosePressed()
		{
			if (ClosePressed != null)
			{
				ClosePressed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
