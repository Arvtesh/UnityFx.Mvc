﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Mvc;

public class AppRoot : MonoBehaviour, IServiceProvider
{
	[SerializeField]
	private Presenter _presenter;
	[SerializeField]
	private ViewFactory _viewFactory;

	private void Awake()
	{
		_presenter.Initialize(this, _viewFactory);
	}

	private async void Start()
	{
		try
		{
			await _presenter.PresentAsync<SplashController>();
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