// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal static class IPresentableExtensions
	{
		public static bool IsChildOf(this IPresentableProxy p, IPresentableProxy other)
		{
			Debug.Assert(other != null);

			while (p != null)
			{
				if (p == other)
				{
					return true;
				}

				p = p.Parent;
			}

			return false;
		}
	}
}
