// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal class PresentResultArgs
	{
		public int Id;
		public int Tag;
		public int Queue;
		
		public PresentResult Parent;
		public Type ControllerType;
		public Type ViewType;
		public Type ResultType;
		public Type ArgsType;
	}
}
