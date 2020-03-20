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
		private ManualUpdateLoop _updateLoop;
		private GameObject _go;
		private IPresentService _presenter;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_viewFactory = new DefaultViewFactory();
			_updateLoop = new ManualUpdateLoop();
			_go = new GameObject("PresenterTests");

			_presenter = new PresenterBuilder(_serviceProvider, _go)
				.UseViewFactory(_viewFactory)
				.UseEventSource(_updateLoop)
				.Build();
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
		public void Present_Calls_OnPresent()
		{
			var presentResult = _presenter.Present<EventsController>();

			Assert.NotNull(presentResult.Controller);
			Assert.AreEqual(1, presentResult.Controller.PresentCallId);
		}

		[Test]
		public void Present_Calls_OnDismiss()
		{
			var presentResult = _presenter.Present<EventsController>();
			presentResult.Dispose();

			Assert.NotNull(presentResult.Controller);
			Assert.AreEqual(2, presentResult.Controller.DismissCallId);
		}

		[Test]
		public void Present_Calls_OnActivate()
		{
			var presentResult = _presenter.Present<EventsController>();
			_updateLoop.Update();

			Assert.NotNull(presentResult.Controller);
			Assert.AreEqual(2, presentResult.Controller.ActivateCallId);
		}

		[Test]
		public void Present_Calls_OnDeactivate()
		{
			var presentResult = _presenter.Present<EventsController>();
			_updateLoop.Update();
			presentResult.Dispose();

			Assert.NotNull(presentResult.Controller);
			Assert.AreEqual(3, presentResult.Controller.DeactivateCallId);
		}

		[Test]
		public void Present_FailsIf_ControllerCtorThrows()
		{
			var presentResult = _presenter.Present<EventsController>(new PresentArgs<ControllerEvents>(ControllerEvents.Ctor));

			Assert.IsEmpty(_presenter.Controllers);
			Assert.NotNull(presentResult);
			Assert.Null(presentResult.Controller);

			Assert.True(presentResult.IsDismissed);
			Assert.True(presentResult.Task.IsFaulted);
			Assert.NotNull(presentResult.Task.Exception);
		}

		[Test]
		public void Present_FailsIf_OnPresentThrows()
		{
			var presentResult = _presenter.Present<EventsController>(new PresentArgs<ControllerEvents>(ControllerEvents.Present));
			_updateLoop.Update();

			Assert.IsEmpty(_presenter.Controllers);
			Assert.NotNull(presentResult);

			Assert.AreEqual(0, presentResult.Controller.ActivateCallId);
			Assert.AreEqual(0, presentResult.Controller.DeactivateCallId);
			Assert.AreEqual(0, presentResult.Controller.DismissCallId);

			Assert.True(presentResult.IsDismissed);
			Assert.True(presentResult.Task.IsFaulted);
			Assert.NotNull(presentResult.Task.Exception);
		}

		[Test]
		public void Present_FailsIf_OnDismissThrows()
		{
			var presentResult = _presenter.Present<EventsController>(new PresentArgs<ControllerEvents>(ControllerEvents.Dismiss));
			presentResult.Dispose();

			Assert.IsEmpty(_presenter.Controllers);
			Assert.NotNull(presentResult);

			Assert.True(presentResult.IsDismissed);
			Assert.True(presentResult.Task.IsFaulted);
			Assert.NotNull(presentResult.Task.Exception);
		}

		[Test]
		public void Present_DoesNotFailIf_OnActivateThrows()
		{
			var presentResult = _presenter.Present<EventsController>(new PresentArgs<ControllerEvents>(ControllerEvents.Activate));
			_updateLoop.Update();
			presentResult.Dispose();

			Assert.AreEqual(1, presentResult.Controller.PresentCallId);
			Assert.AreEqual(2, presentResult.Controller.ActivateCallId);
			Assert.AreEqual(0, presentResult.Controller.DeactivateCallId);
			Assert.AreEqual(3, presentResult.Controller.DismissCallId);

			Assert.True(presentResult.IsDismissed);
			Assert.False(presentResult.Task.IsFaulted);
			Assert.Null(presentResult.Task.Exception);
		}

		[Test]
		public void Present_DoesNotFailIf_OnDeactivateThrows()
		{
			var presentResult = _presenter.Present<EventsController>(new PresentArgs<ControllerEvents>(ControllerEvents.Deactivate));
			_updateLoop.Update();
			presentResult.Dispose();

			Assert.AreEqual(1, presentResult.Controller.PresentCallId);
			Assert.AreEqual(2, presentResult.Controller.ActivateCallId);
			Assert.AreEqual(3, presentResult.Controller.DeactivateCallId);
			Assert.AreEqual(4, presentResult.Controller.DismissCallId);

			Assert.True(presentResult.IsDismissed);
			Assert.False(presentResult.Task.IsFaulted);
			Assert.Null(presentResult.Task.Exception);
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
