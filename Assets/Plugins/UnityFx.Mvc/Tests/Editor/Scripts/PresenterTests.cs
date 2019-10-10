// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace UnityFx.Mvc
{
	[Category("Presenter"), TestOf(typeof(Presenter))]
	public class PresenterTests : IDisposable
	{
		private DefaultViewFactory _viewFactory;
		private DefaultServiceProvider _serviceProvider;
		private Presenter _presenter;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_viewFactory = new DefaultViewFactory();
			_presenter = new Presenter(_serviceProvider, _viewFactory);
		}

		[TearDown]
		public void Dispose()
		{
			_presenter.Dispose();
		}

		[Test]
		public void InitialStateIsValid()
		{
			Assert.IsNull(_presenter.ActiveController);
			Assert.IsEmpty(_presenter.Controllers);
		}

		[Test]
		public void Present_ThrownOnNullControllerType()
		{
			Assert.Throws<ArgumentNullException>(() => _presenter.PresentAsync(null, PresentArgs.Default));
		}

		[Test]
		public void Present_ThrownOnInvalidControllerType()
		{
			Assert.Throws<ArgumentException>(() => _presenter.PresentAsync(typeof(AbstractController), PresentArgs.Default));
			Assert.Throws<ArgumentException>(() => _presenter.PresentAsync(typeof(InvalidController), PresentArgs.Default));
		}

		[Test]
		public void Present_PresentsMinimalController()
		{
			var presentResult = _presenter.PresentAsync<MinimalController>(null);

			Assert.NotNull(presentResult);
		}

		[Test]
		public void Dispose_CanBeCalledMultipleTimes()
		{
			_presenter.Dispose();
			_presenter.Dispose();
		}
	}
}
