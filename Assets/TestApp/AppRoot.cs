// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;
using UnityFx.Mvc.Extensions;

public class AppRoot : MonoBehaviour, IServiceProvider
{
#pragma warning disable 0649

	[SerializeField]
	private UGUIViewFactoryConfig _viewConfig;

#pragma warning restore 0649

	private IViewFactory _viewFactory;
	private IPresenter _presenter;

	private void Awake()
	{
		_viewFactory = new UGUIViewFactoryBuilder(gameObject)
			.UseConfig(_viewConfig)
			.Build();

		_presenter = new PresenterBuilder(this, gameObject)
			.UseViewFactory(_viewFactory)
			.Build();
	}

	private async void Start()
	{
		try
		{
			_presenter.Present<AppController>();

			await _presenter.PresentAsync<SplashController>();

			_presenter.Present<LobbyController>();
			_presenter.PresentMessageBox(MessageBoxOptions.InfoOk, "Welcome to UnityFx.Mvc sample app. This window demonstrates a message box with OK button.", "Info Box");
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
