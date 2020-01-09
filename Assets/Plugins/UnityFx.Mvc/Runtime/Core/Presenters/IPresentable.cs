// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresentable : IPresentResult
	{
		int Layer { get; }
		bool IsActive { get; }
		bool IsDismissed { get; }
		IViewController Controller { get; }
		IPresentable Parent { get; }
		Type ControllerType { get; }
		Task PresentAsync(IViewFactory viewFactory, int index, Transform parent);
		void Update(float frameTime, bool isTop);
		void DismissUnsafe();
		void DisposeUnsafe();
	}
}
