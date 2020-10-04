using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityFx.Mvc;
using UnityFx.Mvc.Extensions;

namespace TestApp.Presentation
{
	/// <summary>
	/// AppController
	/// </summary>
	/// <seealso cref="AppView"/>
	public class AppController : ViewController<AppView>
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AppController"/> class.
		/// </summary>
		public AppController(IPresentContext context)
			: base(context)
		{
			// TODO: Initialize the controller view here. Add arguments to the Configure() as needed.
			View.Configure();
		}

		#endregion

		#region ViewController

		protected override async void OnPresent()
		{
			await Presenter.PresentAsync<SplashController>();
			Presenter.Present<LobbyController>();
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
