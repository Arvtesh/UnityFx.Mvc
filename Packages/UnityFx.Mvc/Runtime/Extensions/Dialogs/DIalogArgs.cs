// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Arguments of a generic dialog.
	/// </summary>
	/// <seealso cref="DialogController"/>
	public class DialogArgs : PresentArgs
	{
		/// <summary>
		/// Gets dialog title text.
		/// </summary>
		/// <seealso cref="Text"/>
		/// <seealso cref="OkText"/>
		/// <seealso cref="CancelText"/>
		public string Title { get; }

		/// <summary>
		/// Gets dialog text (if any).
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
		/// Initializes a new instance of the <see cref="DialogArgs"/> class.
		/// </summary>
		public DialogArgs(string text)
		{
			Text = text;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogArgs"/> class.
		/// </summary>
		public DialogArgs(string text, string title)
		{
			Text = text;
			Title = title;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogArgs"/> class.
		/// </summary>
		public DialogArgs(string text, string title, string okText, string cancelText)
		{
			Text = text;
			Title = title;
			OkText = okText;
			CancelText = cancelText;
		}
	}
}
