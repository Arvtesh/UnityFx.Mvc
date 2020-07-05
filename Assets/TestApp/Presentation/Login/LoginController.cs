using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityFx.Mvc;
using UnityFx.Mvc.Extensions;

namespace TestApp.Presentation
{
	/// <summary>
	/// LoginController
	/// <summary>
	/// <seealso cref="LoginView"/>
	/// <seealso cref="LoginCommands"/>
	[ViewController(Flags = ViewControllerFlags.ModalPopup)]
	public class LoginController : ViewController<LoginView>, ICommandTarget<LoginCommands>
	{
		#region data

		private readonly IWebApi _webApi;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginController"/> class.
		/// <summary>
		public LoginController(IPresentContext context, IWebApi webApi)
			: base(context)
		{
			_webApi = webApi;
		}

		#endregion

		#region ViewController

		/// <inheritdoc/>
		protected override bool OnCommand(Command command, Variant args)
		{
			if (command.TryUnpackEnum(out LoginCommands cmd))
			{
				return InvokeCommand(cmd, args);
			}

			return false;
		}

		#endregion

		#region ICommandTarget

		/// <inheritdoc/>
		public bool InvokeCommand(LoginCommands command, Variant args)
		{
			switch (command)
			{
				case LoginCommands.Login:
					OnLogin(View.Email, View.Password);
					return true;

				case LoginCommands.Dismiss:
					Dismiss();
					return true;
			}

			return false;
		}

		#endregion

		#region implementation

		private async void OnLogin(string email, string password)
		{
			try
			{
				if (_webApi.ActiveUser is null)
				{
					var userInfo = await _webApi.LoginAsync(email, password);
					Debug.Log($"Login successful: {userInfo}.");
				}

				Dismiss();
			}
			catch (Exception e)
			{
				Context.PresentMessageBox(MessageBoxOptions.AlertOk, $"Login failed: {e.Message}.");
			}
		}

		#endregion
	}
}
