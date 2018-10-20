// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.AppStates.Sandbox
{
	/// <summary>
	/// Main menu controller.
	/// </summary>
	public class MainMenuController : ViewController<MainMenuView>
	{
		public MainMenuController(IViewControllerContext context)
			: base(context)
		{
		}
	}
}
