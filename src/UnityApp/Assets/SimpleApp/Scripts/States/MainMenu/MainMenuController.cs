// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.AppStates.Samples
{
	/// <summary>
	/// Main Menu controller.
	/// </summary>
	/// <seealso cref="MainMenuView"/>
	public class MainMenuController : AppViewController
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="MainMenuController"/> class.
		/// </summary>
		public MainMenuController(IPresentableContext context)
			: base(context)
		{
		}

		#endregion

		#region AppViewController

		/// <inheritdoc/>
		public override void OnViewLoaded()
		{
			base.OnViewLoaded();

			var view = View.GetComponent<MainMenuView>();

			if (view)
			{
				view.GamePressed += OnGamePressed;
				view.AboutPressed += OnAboutPressed;
				view.ExitPressed += OnExitPressed;
			}
		}

		public override void OnDismiss()
		{
			var view = View.GetComponent<MainMenuView>();

			if (view)
			{
				view.GamePressed -= OnGamePressed;
				view.AboutPressed -= OnAboutPressed;
				view.ExitPressed -= OnExitPressed;
			}

			base.OnDismiss();
		}

		#endregion

		#region implementation

		private void OnGamePressed(object sender, EventArgs e)
		{
			PresentAsync<GameController>();
		}

		private void OnAboutPressed(object sender, EventArgs e)
		{
			PresentAsync<AboutController>();
		}

		private void OnExitPressed(object sender, EventArgs e)
		{
			Application.Quit();
		}

		#endregion
	}
}
