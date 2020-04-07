// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Arguments of a generic message box.
	/// </summary>
	/// <seealso cref="MessageBoxController"/>
	public class MessageBoxArgs : DialogArgs
	{
		/// <summary>
		/// Gets the message box options.
		/// </summary>
		public MessageBoxOptions Options { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxArgs"/> class.
		/// </summary>
		public MessageBoxArgs(MessageBoxOptions options, string text)
			: base(text)
		{
			Options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxArgs"/> class.
		/// </summary>
		public MessageBoxArgs(MessageBoxOptions options, string text, string title)
			: base(text, title)
		{
			Options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxArgs"/> class.
		/// </summary>
		public MessageBoxArgs(MessageBoxOptions options, string text, string title, string okText, string cancelText)
			: base(text, title, okText, cancelText)
		{
			Options = options;
		}
	}
}
