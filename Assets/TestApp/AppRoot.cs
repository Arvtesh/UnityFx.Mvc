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
	private UGUIMvcConfig _viewConfig;

#pragma warning restore 0649

	private IViewFactory _viewFactory;
	private IPresenter _presenter;

	private void Awake()
	{
		_viewFactory = new UGUIViewServiceBuilder(gameObject)
			.UseConfig(_viewConfig)
			.Build();

		_presenter = new PresenterBuilder(this, gameObject)
			.UseViewFactory(_viewFactory)
			.UseViewControllerBindings(_viewConfig)
			.UseErrorDelegate(OnPresentError)
			.Build();
	}

	private void Start()
	{
		_presenter.Present<AppController>();
	}

	public object GetService(Type serviceType)
	{
		if (serviceType == typeof(IPresenter))
		{
			return _presenter;
		}

		return null;
	}

	private void OnPresentError(Exception e)
	{
		Debug.LogException(e);
	}
}
