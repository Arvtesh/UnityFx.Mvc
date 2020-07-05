using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityFx.Mvc;
using UnityFx.Mvc.Extensions;

namespace TestApp.Presentation
{
	/// <summary>
	/// LobbyController
	/// </summary>
	/// <seealso cref="LobbyView"/>
	[ViewController(Flags = ViewControllerFlags.Exclusive)]
	public class LobbyController : ViewController<LobbyView>
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="LobbyController"/> class.
		/// </summary>
		public LobbyController(IPresentContext context)
			: base(context)
		{
			// TODO: Initialize the controller view here. Add arguments to the Configure() as needed.
			View.Configure();
		}

		#endregion

		#region ViewController

		protected override void OnPresent()
		{
			Context.PresentMessageBox(MessageBoxOptions.InfoOk, "Welcome to UnityFx.Mvc sample app. This window demonstrates a message box with OK button.", "Info Box");
		}

		/// <inheritdoc/>
		protected override bool OnCommand(Command command, Variant args)
		{
			return base.OnCommand(command, args);
		}

		#endregion

		#region implementation
		#endregion
	}

}
