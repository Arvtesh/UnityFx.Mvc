// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// View for the <see cref="MessageBoxController"/>.
	/// </summary>
	/// <seealso cref="MessageBoxController"/>
	public class MessageBoxView : View, IConfigurable<MessageBoxArgs>
	{
		#region data

#pragma warning disable 0649

		[SerializeField]
		private Text _title;
		[SerializeField]
		private Text _text;
		[SerializeField]
		private Text _okText;
		[SerializeField]
		private Button _okButton;
		[SerializeField]
		private Text _cancelText;
		[SerializeField]
		private Button _cancelButton;
		[SerializeField]
		private Button _closeButton;

#pragma warning restore 0649

		#endregion

		#region MonoBehaviour

		private void OnEnable()
		{
			if (_okButton)
			{
				_okButton.onClick.AddListener(OnOk);
			}

			if (_cancelButton)
			{
				_cancelButton.onClick.AddListener(OnCancel);
			}

			if (_closeButton)
			{
				_closeButton.onClick.AddListener(OnClose);
			}
		}

		private void OnDisable()
		{
			if (_okButton)
			{
				_okButton.onClick.RemoveListener(OnOk);
			}

			if (_cancelButton)
			{
				_cancelButton.onClick.RemoveListener(OnCancel);
			}

			if (_closeButton)
			{
				_closeButton.onClick.RemoveListener(OnClose);
			}
		}

		#endregion

		#region IConfigurable

		/// <inheritdoc/>
		public void Configure(MessageBoxArgs args)
		{
			if (_title)
			{
				_title.text = args.Title;
			}

			if (_text)
			{
				_text.text = args.Text;
			}

			if (_okText && !string.IsNullOrEmpty(args.OkText))
			{
				_okText.text = args.OkText;
			}

			if (_cancelText && !string.IsNullOrEmpty(args.CancelText))
			{
				_cancelText.text = args.CancelText;
			}

			if (_okButton)
			{
				_okButton.gameObject.SetActive((args.Options & MessageBoxOptions.Ok) != 0);
			}

			if (_cancelButton)
			{
				_cancelButton.gameObject.SetActive((args.Options & MessageBoxOptions.Cancel) != 0);
			}
		}

		#endregion

		#region implementation

		private void OnOk()
		{
			NotifyCommand(MessageBoxCommands.Ok);
		}

		private void OnCancel()
		{
			NotifyCommand(MessageBoxCommands.Cancel);
		}

		private void OnClose()
		{
			NotifyCommand(MessageBoxCommands.Close);
		}

		#endregion
	}
}
