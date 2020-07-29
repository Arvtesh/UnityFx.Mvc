// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal interface IPresentableProxy : IPresentResult, IViewControllerInfo
	{
		int Layer { get; }
		string PrefabPath { get; }
		bool IsActive { get; }
		IPresentableProxy Parent { get; }
		bool TryActivate();
		void Deactivate();
		Task PresentAsyc(PresentArgs presentArgs);
		void Update(float frameTime);
		void DismissCancel();
		void Dismiss(Exception e);
	}
}
