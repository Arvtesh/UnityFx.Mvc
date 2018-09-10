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
	[ViewResource("AboutDialog")]
	public class AboutController : ViewController<AboutView>
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AboutController"/> class.
		/// </summary>
		public AboutController(IPresentContext context)
			: base(context)
		{
		}

		#endregion

		#region AppViewController

		/// <inheritdoc/>
		public override void OnViewLoaded()
		{
			base.OnViewLoaded();

			ViewAspect.ClosePressed += OnClosePressed;
		}

		/// <inheritdoc/>
		public override void OnDismiss()
		{
			ViewAspect.ClosePressed -= OnClosePressed;

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
