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
		bool IsActive { get; }
		bool IsDismissed { get; }
		IPresentable Parent { get; }
		PresentOptions PresentOptions { get; }
		Task PresentAsync(IViewFactory viewFactory, int index, Transform parent);
		void Update(float frameTime, bool isTop);
		void DismissUnsafe();
		void DisposeUnsafe();
	}
}
