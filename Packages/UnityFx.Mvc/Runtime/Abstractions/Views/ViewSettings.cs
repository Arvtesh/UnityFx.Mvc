// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CreateAssetMenu(fileName = "ViewSettings", menuName = "UnityFx/Mvc/View Settings")]
	public class ViewSettings : ScriptableObject
	{
		#region data

#pragma warning disable 0649

		[SerializeField, Range(0, 255)]
		private int _layer;

#pragma warning restore 0649

		#endregion
	}
}
