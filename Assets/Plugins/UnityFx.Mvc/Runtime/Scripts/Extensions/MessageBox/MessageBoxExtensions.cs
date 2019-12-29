// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Extensions for <see cref="MessageBoxController"/>-related entities.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MessageBoxExtensions
	{
		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/>.
		/// </summary>
		public static IPresentResult<MessageBoxResult> PresentMessageBox(this IPresenter presenter, MessageBoxOptions options, string text)
		{
			return (IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text));
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/>.
		/// </summary>
		public static Task<MessageBoxResult> PresentMessageBoxAsync(this IPresenter presenter, MessageBoxOptions options, string text)
		{
			return ((IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text))).Task;
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/> and <paramref name="title"/>.
		/// </summary>
		public static IPresentResult<MessageBoxResult> PresentMessageBox(this IPresenter presenter, MessageBoxOptions options, string text, string title)
		{
			return (IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title));
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/> and <paramref name="title"/>.
		/// </summary>
		public static Task<MessageBoxResult> PresentMessageBoxAsync(this IPresenter presenter, MessageBoxOptions options, string text, string title)
		{
			return ((IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title))).Task;
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/> and <paramref name="title"/>.
		/// </summary>
		public static IPresentResult<MessageBoxResult> PresentMessageBox(this IPresenter presenter, MessageBoxOptions options, string text, string title, string okText, string cancelText)
		{
			return (IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title, okText, cancelText));
		}

		/// <summary>
		/// Presents a message box with the specified <paramref name="text"/> and <paramref name="title"/>.
		/// </summary>
		public static Task<MessageBoxResult> PresentMessageBoxAsync(this IPresenter presenter, MessageBoxOptions options, string text, string title, string okText, string cancelText)
		{
			return ((IPresentResult<MessageBoxResult>)presenter.Present(typeof(MessageBoxController), new MessageBoxArgs(options, text, title, okText, cancelText))).Task;
		}
	}
}
