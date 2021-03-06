﻿// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal class PresentResultArgs
	{
		public IServiceProvider ServiceProvider;
		public IViewControllerFactory ControllerFactory;
		public IViewFactory ViewFactory;

		public int Id;
		public int Layer;
		public int Tag;
		public string PrefabPath;
		
		public IPresentable Parent;
		public Type ControllerType;
		public PresentOptions PresentOptions;
		public PresentArgs PresentArgs;
	}
}
