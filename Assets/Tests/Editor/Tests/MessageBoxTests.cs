// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using UnityFx.Mvc.Extensions;
using NUnit.Framework;

namespace UnityFx.Mvc
{
	[Category("MessageBox"), TestOf(typeof(MessageBoxController))]
	public class MessageBoxTests : IDisposable
	{
		private GameObject _go;
		private DefaultServiceProvider _serviceProvider;
		private IViewFactory _viewFactory;
		private Presenter _presenter;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_viewFactory = new DefaultViewFactory();
			_go = new GameObject("MessageBoxTest");
			_presenter = new PresenterBuilder(_serviceProvider, _go).UseViewFactory(_viewFactory).Build();
		}

		[TearDown]
		public void Dispose()
		{
			_presenter.Dispose();
		}

		[Test]
		public void PresentMessageBox_Functions()
		{
			_presenter.PresentMessageBox(MessageBoxOptions.InfoOk, "Test");
		}
	}
}
