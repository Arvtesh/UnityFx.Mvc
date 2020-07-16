// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Message box related extensions.
	/// </summary>
	/// <seealso cref="MessageBoxController"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MessageBoxExtensions
	{
		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/> and <paramref name="options"/>.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="options">Flags that define the popup style and behavior.</param>
		/// <param name="text">Text to display in the popup.</param>
		/// <seealso cref="PresentMessageBoxAsync(IPresenter, MessageBoxOptions, string)"/>
		/// <seealso cref="MessageBoxController"/>
		public static IPresentResult<MessageBoxResult> PresentMessageBox(this IPresenter presenter, MessageBoxOptions options, string text)
		{
			return (IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text));
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/> and <paramref name="options"/>.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="options">Flags that define the popup style and behavior.</param>
		/// <param name="text">Text to display in the popup.</param>
		/// <seealso cref="PresentMessageBox(IPresenter, MessageBoxOptions, string)"/>
		/// <seealso cref="MessageBoxController"/>
		public static Task<MessageBoxResult> PresentMessageBoxAsync(this IPresenter presenter, MessageBoxOptions options, string text)
		{
			return presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text)).GetResultAsync<MessageBoxResult>();
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/>, <paramref name="title"/> and <paramref name="options"/>.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="options">Flags that define the popup style and behavior.</param>
		/// <param name="text">Text to display in the popup.</param>
		/// <param name="title">The popup title text (can be set to <see langword="null"/> to hide title).</param>
		/// <seealso cref="PresentMessageBoxAsync(IPresenter, MessageBoxOptions, string, string)"/>
		/// <seealso cref="MessageBoxController"/>
		public static IPresentResult<MessageBoxResult> PresentMessageBox(this IPresenter presenter, MessageBoxOptions options, string text, string title)
		{
			return (IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title));
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/>, <paramref name="title"/> and <paramref name="options"/>.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="options">Flags that define the popup style and behavior.</param>
		/// <param name="text">Text to display in the popup.</param>
		/// <param name="title">The popup title text (can be set to <see langword="null"/> to hide title).</param>
		/// <seealso cref="PresentMessageBox(IPresenter, MessageBoxOptions, string, string)"/>
		/// <seealso cref="MessageBoxController"/>
		public static Task<MessageBoxResult> PresentMessageBoxAsync(this IPresenter presenter, MessageBoxOptions options, string text, string title)
		{
			return presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title)).GetResultAsync<MessageBoxResult>();
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/>, <paramref name="title"/> and <paramref name="options"/>.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="options">Flags that define the popup style and behavior.</param>
		/// <param name="text">Text to display in the popup.</param>
		/// <param name="title">The popup title text (can be set to <see langword="null"/> to hide title).</param>
		/// <param name="okText">Text of the popup OK button (if any).</param>
		/// <param name="cancelText">Text of the popup CANCEL button (if any).</param>
		/// <seealso cref="PresentMessageBoxAsync(IPresenter, MessageBoxOptions, string, string, string, string)"/>
		/// <seealso cref="MessageBoxController"/>
		public static IPresentResult<MessageBoxResult> PresentMessageBox(this IPresenter presenter, MessageBoxOptions options, string text, string title, string okText, string cancelText)
		{
			return (IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title, okText, cancelText));
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/>, <paramref name="title"/> and <paramref name="options"/>.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="options">Flags that define the popup style and behavior.</param>
		/// <param name="text">Text to display in the popup.</param>
		/// <param name="title">The popup title text (can be set to <see langword="null"/> to hide title).</param>
		/// <param name="okText">Text of the popup OK button (if any).</param>
		/// <param name="cancelText">Text of the popup CANCEL button (if any).</param>
		/// <seealso cref="PresentMessageBox(IPresenter, MessageBoxOptions, string, string, string, string)"/>
		/// <seealso cref="MessageBoxController"/>
		public static Task<MessageBoxResult> PresentMessageBoxAsync(this IPresenter presenter, MessageBoxOptions options, string text, string title, string okText, string cancelText)
		{
			return presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title, okText, cancelText)).GetResultAsync<MessageBoxResult>();
		}
	}
}
