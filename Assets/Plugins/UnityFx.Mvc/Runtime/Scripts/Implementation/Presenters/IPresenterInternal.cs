// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresenterInternal
	{
		IServiceProvider ServiceProvider { get; }
		IPresentResult PresentAsync(IPresentable presentable, Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args);
		void Dismiss(IPresentable presentable);
		void Remove(IPresentable presentable);
		int GetNextId();
	}
}
