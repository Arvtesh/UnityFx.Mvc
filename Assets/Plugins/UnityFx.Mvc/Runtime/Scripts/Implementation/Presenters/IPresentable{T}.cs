// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresentable<out T> : IPresentable, IPresentResult<T> where T : IViewController
	{
	}
}
