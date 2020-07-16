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
			Debug.Assert(_presenter is null);
			_presenter = presenter;
		}

		#endregion

		#region MonoBehaviour

		private void Update()
		{
			if (_presenter != null && _presenter.NeedEventSource)
			{
				_presenter.OnUpdate();
			}
		}

		private void OnDestroy()
		{
			_presenter?.Dispose();
		}

		private void OnApplicationQuit()
		{
			// NOTE: Do not dispose presenter when the app is being shout down.
			_presenter = null;
		}

		#endregion
	}
}
