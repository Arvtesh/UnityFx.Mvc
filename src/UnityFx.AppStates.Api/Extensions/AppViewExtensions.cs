// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Extension methods for <see cref="IAppView"/>.
	/// </summary>
	public static class AppViewExtensions
	{
		/// <summary>
		/// Searches the view root for the specified component. Returns <see langword="null"/> if no components found.
		/// </summary>
		public static T GetComponent<T>(this IAppView view) where T : class
		{
			foreach (var go in view)
			{
				var c = go.GetComponent<T>();

				if (c != null)
				{
					return c;
				}
			}

			return null;
		}

		/// <summary>
		/// Searches the view root for the specified components. Returns an empty array if no components found.
		/// </summary>
		public static T[] GetComponents<T>(this IAppView view) where T : class
		{
			var result = new List<T>();

			foreach (var go in view)
			{
				go.GetComponents(result);
			}

			return result.ToArray();
		}

		/// <summary>
		/// Searches the view for the specified component recursively. Returns <see langword="null"/> if no components found.
		/// </summary>
		public static T GetComponentRecursive<T>(this IAppView view) where T : class
		{
			foreach (var go in view)
			{
				var c = go.GetComponentInChildren<T>(true);

				if (c != null)
				{
					return c;
				}
			}

			return null;
		}

		/// <summary>
		/// Searches the view for the specified components recursively. Returns an empty array if no components found.
		/// </summary>
		public static T[] GetComponentsRecursive<T>(this IAppView view) where T : class
		{
			var result = new List<T>();

			foreach (var go in view)
			{
				go.GetComponentsInChildren(true, result);
			}

			return result.ToArray();
		}
	}
}
