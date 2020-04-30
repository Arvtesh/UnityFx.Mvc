// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Configuration of UGUI-based <see cref="IViewFactory"/> implementations.
	/// </summary>
	/// <seealso cref="UGUIViewServiceBuilder"/>
	[CreateAssetMenu(fileName = "MvcConfig", menuName = "UnityFx/Mvc/MVC Config (UGUI)")]
	public sealed class UGUIMvcConfig : MvcConfig
	{
		#region data

#pragma warning disable 0649
#pragma warning restore 0649

		#endregion

		#region interface

#if UNITY_EDITOR

		internal const string DefaultViewPath = "Packages/com.unityfx.mvc.ugui/Runtime/Views/UGUIView.cs";

#endif

		#endregion
	}
}
