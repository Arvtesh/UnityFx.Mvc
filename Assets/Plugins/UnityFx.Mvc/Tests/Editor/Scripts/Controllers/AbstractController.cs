﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	public abstract class AbstractController : IViewController
	{
		public IView View => null;

		public bool InvokeCommand(string commandName, object args)
		{
			return false;
		}
	}
}