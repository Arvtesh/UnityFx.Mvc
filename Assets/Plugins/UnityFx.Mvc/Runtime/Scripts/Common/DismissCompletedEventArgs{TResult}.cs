// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event that signals of the dismiss operation completion.
	/// </summary>
	public class DismissCompletedEventArgs<TResult> : AsyncCompletedEventArgs
	{
		/// <summary>
		/// Gets the controller result.
		/// </summary>
		public TResult Result { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="result">The controller result.</param>
		/// <param name="userState">User-defined state assosiated with the operation.</param>
		public DismissCompletedEventArgs(TResult result, object userState)
			: base(null, false, userState)
		{
			Result = result;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="error">The present error.</param>
		/// <param name="userState">User-defined state assosiated with the operation.</param>
		public DismissCompletedEventArgs(Exception error, object userState)
			: base(error, error is OperationCanceledException, userState)
		{
		}
	}
}
