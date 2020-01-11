// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;

public class AppRoot : MonoBehaviour, IServiceProvider
{
	private IPresenter _presenter;

	private void Awake()
	{
		_presenter = new PresenterBuilder(this, gameObject).Build();
	}

	private async void Start()
	{
		try
		{
			_presenter.Present<AppController>();

			await _presenter.PresentAsync<SplashController>();

			_presenter.Present<LobbyController>();
		}
		catch (OperationCanceledException)
		{
			// do nothing
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}

	public object GetService(Type serviceType)
	{
		if (serviceType == typeof(IPresenter))
		{
			return _presenter;
		}

		return null;
	}
}
