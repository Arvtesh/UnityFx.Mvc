// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementatino of <see cref="IAppViewFactory"/>.
	/// </summary>
	internal class AppViewLayer : DisposableComponentList<IAppView>, IAppViewLayer
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region MonoBehaviour
		#endregion

		#region IAppViewLayer

		public IReadOnlyList<IAppView> Views
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		#endregion

		#region IAppViewFactory

		public IAppView CreateView(string name, IAppView insertAfter)
		{
			ThrowIfDisposed();

			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Invalid view name", nameof(name));
			}

			var viewGo = new GameObject(name);
			var viewTransform = viewGo.transform;
			viewTransform.SetParent(transform, false);

			if (insertAfter is AppView iav)
			{
				var siblingIndex = iav.transform.GetSiblingIndex();
				viewTransform.SetSiblingIndex(siblingIndex + 1);
			}

			return viewGo.AddComponent<AppView>();
		}

		#endregion

		#region implementation
		#endregion
	}
}
