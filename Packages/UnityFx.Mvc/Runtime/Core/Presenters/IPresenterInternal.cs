// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresenterInternal
	{
		IEnumerable<IPresentableProxy> GetChildren(IPresentableProxy presentable);
		IPresentResult PresentAsync(IPresentableProxy presentable, Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args);
		void PresentCompleted(IPresentableProxy presentable, Exception e, bool cancelled);
		void ReportError(Exception e);
	}
}
