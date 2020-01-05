// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;

public class AppRoot : MonoBehaviour, IServiceProvider
{
	[SerializeField]
	private Presenter _presenter;
	[SerializeField]
	private UGUIViewFactory _viewFactory;

	private void Awake()
	{
		if (_presenter is null)
		{
			_presenter = gameObject.AddComponent<Presenter>();
		}

		if (_viewFactory is null)
		{
			_viewFactory = gameObject.AddComponent<UGUIViewFactory>();
		}

		_presenter.Initialize(this);
	}

	private async void Start()
	{
		try
		{
			_ = _presenter.PresentAsync<AppController>();

			await _presenter.PresentAsync<SplashController>();

			_ = _presenter.PresentAsync<LobbyController>();
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
		if (serviceType == typeof(IViewFactory))
		{
			return _viewFactory;
		}

		if (serviceType == typeof(IPresenter))
		{
			return _presenter;
		}

		return null;
	}
}
