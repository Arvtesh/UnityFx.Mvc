using System;
using UnityEngine;
using UnityEngine.UI;
using UnityFx.Mvc;

namespace TestApp.Presentation
{
	/// <summary>
	/// View for the <see cref="LoginController"/>.
	/// <summary>
	/// <seealso cref="LoginController"/>
	/// <seealso cref="LoginCommands"/>
	public class LoginView : UGUIView
	{
		#region data

#pragma warning disable 0649

		[SerializeField]
		private InputField _email;
		[SerializeField]
		private InputField _password;
		[SerializeField]
		private Button _loginButton;

#pragma warning restore 0649

		#endregion

		#region interface

		public string Email => _email.text;

		public string Password => _password.text;

		/// <summary>
		/// Initializes the view. Called from the controller ctor.
		/// <summary>
		public void Configure()
		{
			// TODO: Setup the view here. Add additional arguments as needed.
		}

		#endregion

		#region MonoBehaviour

		private void OnEnable()
		{
			_loginButton.onClick.AddListener(OnLogin);
			_email.onValueChanged.AddListener(OnEmailChanged);
			_loginButton.interactable = false;
		}

		private void OnDisable()
		{
			_email.onValueChanged.RemoveListener(OnEmailChanged);
			_loginButton.onClick.RemoveListener(OnLogin);
		}

		#endregion

		#region implementation

		private void OnEmailChanged(string value)
		{
			_loginButton.interactable = !string.IsNullOrEmpty(value) && value.Contains("@");
		}

		private void OnLogin()
		{
			NotifyCommand(LoginCommands.Login);
		}

		#endregion
	}
}
