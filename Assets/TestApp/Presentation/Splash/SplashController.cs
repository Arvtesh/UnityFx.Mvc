// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;

namespace TestApp.Presentation
{
	public class SplashController : ViewController
	{
		private float _timer;

		public SplashController(IPresentContext context)
			: base(context)
		{
		}

		protected override void OnUpdate()
		{
			_timer += Time.deltaTime;

			if (_timer > 3)
			{
				Dismiss();
			}
		}
	}
}
