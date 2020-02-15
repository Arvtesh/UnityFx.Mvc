// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresenterInternal
	{
		IEnumerable<IPresentable> GetChildren(IPresentable presentable);
		IPresentResult PresentAsync(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args);
	}
}
