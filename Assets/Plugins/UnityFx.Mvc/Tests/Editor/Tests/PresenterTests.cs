// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace UnityFx.Mvc
{
	[Category("Presenter"), TestOf(typeof(IPresentService))]
	public class PresenterTests : IDisposable
	{
		private DefaultViewFactory _viewFactory;
		private DefaultServiceProvider _serviceProvider;
		private GameObject _go;
		private IPresentService _presenter;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_viewFactory = new DefaultViewFactory();
			_go = new GameObject("PresenterTests");
			_presenter = new PresenterBuilder(_serviceProvider, _go).UseViewFactory(_viewFactory).Build();
		}

		[TearDown]
		public void Dispose()
		{
			GameObject.DestroyImmediate(_go);
		}

		[Test]
		public void InitialStateIsValid()
		{
			Assert.AreEqual(_serviceProvider, _presenter.ServiceProvider);
			Assert.AreEqual(_viewFactory, _presenter.ViewFactory);
			Assert.IsNull(_presenter.ActiveController);
			Assert.IsEmpty(_presenter.Controllers);
		}

		[Test]
		public void Present_ThrowsOnNullControllerType()
		{
			Assert.Throws<ArgumentNullException>(() => _presenter.Present(null));
		}

		[Test]
		public void Present_ThrowsOnAbstractControllerType()
		{
			Assert.Throws<ArgumentException>(() => _presenter.Present(typeof(AbstractController)));
		}

		[Test]
		public void Present_ThrowsOnInvalidControllerType()
		{
			Assert.Throws<ArgumentException>(() => _presenter.Present(typeof(InvalidController)));
		}

		[Test]
		public void Present_PresentsMinimalController()
		{
			var presentResult = _presenter.Present<MinimalController>();

			Assert.NotNull(presentResult);
			Assert.False(presentResult.IsDismissed);
			Assert.AreEqual(typeof(MinimalController), presentResult.ControllerType);
		}

		[Test]
		public void Present_MaintainsTheOnlyInstanceOfSingletonController()
		{
			var presentResult = _presenter.Present<SingletonController>();
			var presentResult2 = _presenter.Present<SingletonController>();

			Assert.True(presentResult.IsDismissed);
			Assert.False(presentResult2.IsDismissed);
			Assert.AreEqual(1, _presenter.Controllers.Count);
			Assert.AreEqual(presentResult2.Controller, _presenter.Controllers.Peek());
		}

		[Test]
		public void Dispose_CanBeCalledMultipleTimes()
		{
			_presenter.Dispose();
			_presenter.Dispose();
		}
	}
}
