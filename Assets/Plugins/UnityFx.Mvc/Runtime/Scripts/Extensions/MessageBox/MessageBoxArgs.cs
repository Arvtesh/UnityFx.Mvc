// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// A <see cref="MessageBoxController"/> present arguments.
	/// </summary>
	public class MessageBoxArgs : PresentArgs
	{
		/// <summary>
		/// Gets message box title text.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets message box text.
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Gets OK button text.
		/// </summary>
		public string OkText { get; }

		/// <summary>
		/// Gets CANCEL button text.
		/// </summary>
		public string CancelText { get; }

		/// <summary>
		/// Gets the message box options.
		/// </summary>
		public MessageBoxOptions Options { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxArgs"/> class.
		/// </summary>
		public MessageBoxArgs(MessageBoxOptions options, string text)
		{
			Text = text;
			Options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxArgs"/> class.
		/// </summary>
		public MessageBoxArgs(MessageBoxOptions options, string text, string title)
		{
			Text = text;
			Title = title;
			Options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxArgs"/> class.
		/// </summary>
		public MessageBoxArgs(MessageBoxOptions options, string text, string title, string okText, string cancelText)
		{
			Text = text;
			Title = title;
			OkText = okText;
			CancelText = cancelText;
			Options = options;
		}
	}
}
