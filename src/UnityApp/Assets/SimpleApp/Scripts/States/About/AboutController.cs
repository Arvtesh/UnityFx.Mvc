// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.AppStates.Samples
{
	/// <summary>
	/// About dialog controller.
	/// </summary>
	/// <seealso cref="AboutView"/>
	public class AboutController : AppViewController
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AboutController"/> class.
		/// </summary>
		public AboutController(IPresentableContext context)
			: base(context)
		{
		}

		#endregion

		#region AppViewController

		/// <inheritdoc/>
		public override void OnViewLoaded()
		{
			base.OnViewLoaded();

			var view = View.GetComponent<AboutView>();

			if (view)
			{
				view.ClosePressed += OnClosePressed;
			}
		}

		/// <inheritdoc/>
		public override void OnDismiss()
		{
			var view = View.GetComponent<AboutView>();

			if (view)
			{
				view.ClosePressed -= OnClosePressed;
			}

			base.OnDismiss();
		}

		#endregion

		#region implementation

		private void OnClosePressed(object sender, EventArgs e)
		{
			DismissAsync();
		}

		#endregion
	}
}
