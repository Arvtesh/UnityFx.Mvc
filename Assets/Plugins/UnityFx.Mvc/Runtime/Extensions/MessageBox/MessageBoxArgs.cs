// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Arguments of a generic message box.
	/// </summary>
	/// <seealso cref="MessageBoxController"/>
	public class MessageBoxArgs : PresentArgs
	{
		/// <summary>
		/// Gets message box title text.
		/// </summary>
		/// <seealso cref="Text"/>
		/// <seealso cref="OkText"/>
		/// <seealso cref="CancelText"/>
		public string Title { get; }

		/// <summary>
		/// Gets message box text.
		/// </summary>
		/// <seealso cref="Title"/>
		/// <seealso cref="OkText"/>
		/// <seealso cref="CancelText"/>
		public string Text { get; }

		/// <summary>
		/// Gets OK button text.
		/// </summary>
		/// <seealso cref="CancelText"/>
		/// <seealso cref="Title"/>
		/// <seealso cref="Text"/>
		public string OkText { get; }

		/// <summary>
		/// Gets CANCEL button text.
		/// </summary>
		/// <seealso cref="OkText"/>
		/// <seealso cref="Title"/>
		/// <seealso cref="Text"/>
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
