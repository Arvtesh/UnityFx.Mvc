// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal class PresenterBehaviour : MonoBehaviour
	{
		#region data

		private Presenter _presenter;

		#endregion

		#region interface

		public IPresentService Presenter => _presenter;

		internal void Initialize(Presenter presenter)
		{
			Debug.Assert(_presenter == null);
			_presenter = presenter;
		}

		#endregion

		#region MonoBehaviour

		private void Update()
		{
			_presenter?.Update();
		}

		private void OnDestroy()
		{
			_presenter?.Dispose();
		}

		#endregion
	}
}
