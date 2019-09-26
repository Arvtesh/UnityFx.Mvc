// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event that signals of the present operation completion.
	/// </summary>
	public class PresentCompletedEventArgs : AsyncCompletedEventArgs
	{
		/// <summary>
		/// Gets the controller presented.
		/// </summary>
		public IViewController Controller { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="controller">The controller presented.</param>
		/// <param name="userState">User-defined state assosiated with the operation.</param>
		public PresentCompletedEventArgs(IViewController controller, object userState)
			: base(null, false, userState)
		{
			Controller = controller;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="error">The present error.</param>
		/// <param name="userState">User-defined state assosiated with the operation.</param>
		public PresentCompletedEventArgs(Exception error, object userState)
			: base(error, error is OperationCanceledException, userState)
		{
		}
	}
}
