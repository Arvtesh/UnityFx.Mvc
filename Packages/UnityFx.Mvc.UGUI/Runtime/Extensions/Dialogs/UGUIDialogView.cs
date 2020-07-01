// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// UGUI view of a generic dialog.
	/// </summary>
	public abstract class UGUIDialogView : UGUIView
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

		#region interface

		protected Text TitleText => _okText;

		protected Text ContentText => _okText;

		protected Button OkButton => _okButton;

		protected Text OkText => _okText;

		protected Button CancelButton => _cancelButton;

		protected Text CancelText => _cancelText;

		protected Button CloseButton => _closeButton;

		protected void Configure(DialogArgs args)
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
		}

		protected abstract void OnOk();

		protected abstract void OnCancel();

		protected abstract void OnClose();

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

		#region implementation
		#endregion
	}
}
