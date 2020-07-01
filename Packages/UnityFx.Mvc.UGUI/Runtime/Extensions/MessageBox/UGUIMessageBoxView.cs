// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// UGUI view for <see cref="MessageBoxController"/>.
	/// </summary>
	/// <seealso cref="MessageBoxController"/>
	[ViewControllerBinding(typeof(MessageBoxController))]
	public sealed class UGUIMessageBoxView : UGUIDialogView, IConfigurable<MessageBoxArgs>
	{
		#region UGUIDialogView

		protected override void OnOk()
		{
			NotifyCommand(MessageBoxCommands.Ok);
		}

		protected override void OnCancel()
		{
			NotifyCommand(MessageBoxCommands.Cancel);
		}

		protected override void OnClose()
		{
			NotifyCommand(MessageBoxCommands.Close);
		}

		#endregion

		#region IConfigurable

		/// <inheritdoc/>
		public void Configure(MessageBoxArgs args)
		{
			base.Configure(args);

			if (OkButton)
			{
				OkButton.gameObject.SetActive((args.Options & MessageBoxOptions.Ok) != 0);
			}

			if (CancelButton)
			{
				CancelButton.gameObject.SetActive((args.Options & MessageBoxOptions.Cancel) != 0);
			}
		}

		#endregion
	}
}
