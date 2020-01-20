// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Arguments for the <see cref="LoadingController"/>.
	/// </summary>
	/// <seealso cref="LoadingController"/>
	public class LoadingArgs : PresentArgs
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
		/// Gets CANCEL button text.
		/// </summary>
		public string CancelText { get; }

		/// <summary>
		/// Called to initiate the asynchronous operation.
		/// </summary>
		public Func<IProgress<float>, CancellationToken, Task> TaskFactory { get; }

		/// <summary>
		/// Called to initiate the asynchronous operation.
		/// </summary>
		public Func<CancellationToken, AsyncOperation> AsyncOperationFactory { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadingArgs"/> class.
		/// </summary>
		public LoadingArgs(string text, string title, string cancelText, Func<IProgress<float>, CancellationToken, Task> taskFactory)
		{
			Text = text;
			Title = title;
			CancelText = cancelText;
			TaskFactory = taskFactory;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadingArgs"/> class.
		/// </summary>
		public LoadingArgs(string text, string title, string cancelText, Func<CancellationToken, AsyncOperation> asyncFactory)
		{
			Text = text;
			Title = title;
			CancelText = cancelText;
			AsyncOperationFactory = asyncFactory;
		}
	}
}
