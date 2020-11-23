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
	[Category("PresentResult"), TestOf(typeof(IPresentResult))]
	public class PresentResultTests : IDisposable
	{
		private DefaultViewFactory _viewFactory;
		private DefaultServiceProvider _serviceProvider;
		private Presenter _presenter;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_viewFactory = new DefaultViewFactory();
			_presenter = new PresenterBuilder(_serviceProvider, new GameObject("PresentResultTests")).UseViewFactory(_viewFactory).Build();
		}

		[TearDown]
		public void Dispose()
		{
			_presenter.Dispose();
		}

		[Test]
		public void Tag_IsValid()
		{
			var presentResult = _presenter.Present<TagController>();
			//Assert.AreEqual(TagController.TagValue, presentResult.Tag);
		}

		[Test]
		public void Dispose_DismissesController()
		{
			var presentResult = _presenter.Present<MinimalController>();

			presentResult.Dispose();

			Assert.True(presentResult.IsDismissed);
			Assert.IsEmpty(_presenter.Controllers);
		}
	}
}
