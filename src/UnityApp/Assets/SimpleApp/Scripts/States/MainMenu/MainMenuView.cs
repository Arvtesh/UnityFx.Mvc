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
	/// <seealso cref="MainMenuController"/>
	public class MainMenuView : MonoBehaviour
	{
		#region data

		[SerializeField]
		private Button _gameButton;
		[SerializeField]
		private Button _aboutButton;
		[SerializeField]
		private Button _exitButton;

		#endregion

		#region interface

		/// <summary>
		/// Raised when Game button is pressed.
		/// </summary>
		public event EventHandler GamePressed;

		/// <summary>
		/// Raised when About button is pressed.
		/// </summary>
		public event EventHandler AboutPressed;

		/// <summary>
		/// Raised when Exit button is pressed.
		/// </summary>
		public event EventHandler ExitPressed;

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			if (_gameButton)
			{
				_gameButton.onClick.AddListener(OnGamePressed);
			}

			if (_aboutButton)
			{
				_aboutButton.onClick.AddListener(OnAboutPressed);
			}

			if (_exitButton)
			{
				_exitButton.onClick.AddListener(OnExitPressed);
			}
		}

		#endregion

		#region implementation

		private void OnGamePressed()
		{
			if (GamePressed != null)
			{
				GamePressed(this, EventArgs.Empty);
			}
		}

		private void OnAboutPressed()
		{
			if (AboutPressed != null)
			{
				AboutPressed(this, EventArgs.Empty);
			}
		}

		private void OnExitPressed()
		{
			if (ExitPressed != null)
			{
				ExitPressed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
