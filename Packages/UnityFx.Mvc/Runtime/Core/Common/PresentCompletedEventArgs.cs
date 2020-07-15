// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// TODO
	/// </summary>
	public class PresentCompletedEventArgs : AsyncCompletedEventArgs
	{
		/// <summary>
		/// Gets the present info.
		/// </summary>
		public IViewControllerInfo PresentInfo { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentCompletedEventArgs"/> class.
		/// </summary>
		public PresentCompletedEventArgs(IViewControllerInfo controllerInfo, Exception error, bool cancelled)
			: base(error, cancelled || error is OperationCanceledException, controllerInfo)
		{
			PresentInfo = controllerInfo;
		}
	}
}
