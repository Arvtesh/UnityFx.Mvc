// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Enumerates flags that can be used to specify a message box style and behavior.
	/// </summary>
	/// <seealso cref="MessageBoxResult"/>
	/// <seealso cref="MessageBoxController"/>
	[Flags]
	public enum MessageBoxOptions
	{
		/// <summary>
		/// No options.
		/// </summary>
		None = 0,

		/// <summary>
		/// Message box with OK button.
		/// </summary>
		Ok = 1,

		/// <summary>
		/// Message box with OK button.
		/// </summary>
		Cancel = 2,

		/// <summary>
		/// Message box with OK and CANCEL buttons.
		/// </summary>
		OkCancel = Ok | Cancel,

		/// <summary>
		/// Info box style.
		/// </summary>
		Info = 4,

		/// <summary>
		/// Info box style with OK button.
		/// </summary>
		InfoOk = Info | Ok,

		/// <summary>
		/// Info box style with OK and Cancel buttons.
		/// </summary>
		InfoOkCancel = Info | Ok | Cancel,

		/// <summary>
		/// Warning style.
		/// </summary>
		Warning = 8,

		/// <summary>
		/// Warning style with OK button.
		/// </summary>
		WarningOk = Warning | Ok,

		/// <summary>
		/// Warning style with OK and Cancel buttons.
		/// </summary>
		WarningOkCancel = Warning | Ok | Cancel,

		/// <summary>
		/// Error style.
		/// </summary>
		Error = 16,

		/// <summary>
		/// Error style with OK button.
		/// </summary>
		ErrorOk = Error | Ok,

		/// <summary>
		/// Error style with OK and Cancel buttons.
		/// </summary>
		ErrorOkCancel = Error | Ok | Cancel,

		/// <summary>
		/// Alert style.
		/// </summary>
		Alert = 32,

		/// <summary>
		/// Alert style with OK button.
		/// </summary>
		AlertOk = Alert | Ok,

		/// <summary>
		/// Alert style with OK and Cancel buttons.
		/// </summary>
		AlertOkCancel = Alert | Ok | Cancel,
	}
}
