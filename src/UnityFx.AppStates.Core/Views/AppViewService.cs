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
	/// Implementatino of <see cref="IAppViewService"/>.
	/// </summary>
	internal class AppViewService : DisposableComponentList<IAppViewLayer>, IAppViewService
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region MonoBehaviour
		#endregion

		#region IAppViewService

		public IReadOnlyList<IAppViewLayer> Layers
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		public IAppViewLayer AddLayer()
		{
			ThrowIfDisposed();

			var go = new GameObject("Layer" + transform.childCount.ToString());
			go.transform.SetParent(transform, false);
			return go.AddComponent<AppViewLayer>();
		}

		public IAppViewLayer AddLayer(int index)
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		#endregion

		#region implementation
		#endregion
	}
}
