// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;

public class AppController : ViewController
{
	public AppController(IPresentContext context)
		: base(context)
	{
	}

	protected override void OnPresent()
	{
		Context.PresentAsync<SplashController>();
	}
}
