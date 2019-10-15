// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	internal interface IPresentable : IPresentResult, ICommandTarget
	{
		bool IsActive { get; }
		IPresentable Parent { get; }
		Task PresentAsync(IViewFactory viewFactory, int index);
		void Update(float frameTime, bool isTop);
		void DismissUnsafe();
		void DisposeUnsafe();
	}
}
