// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;

[ViewController(Flags = ViewControllerFlags.Exclusive)]
public class SplashController : ViewController
{
	public SplashController(IPresentContext context)
		: base(context)
	{
		context.Schedule(OnTimer, 5);
	}

	private void OnTimer(float t)
	{
		Dismiss();
	}
}
