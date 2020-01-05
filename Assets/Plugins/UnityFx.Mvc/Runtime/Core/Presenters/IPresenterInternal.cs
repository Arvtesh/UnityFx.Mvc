// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresenterInternal
	{
		IPresentResult PresentAsync(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args);
		void Dismiss(IPresentable presentable);
	}
}
