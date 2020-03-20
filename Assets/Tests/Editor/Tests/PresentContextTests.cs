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
	[Category("PresentContext"), TestOf(typeof(IPresentContext))]
	public class PresentContextTests : IDisposable
	{
		private DefaultViewFactory _viewFactory;
		private DefaultServiceProvider _serviceProvider;
		private IPresentService _presenter;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_viewFactory = new DefaultViewFactory();
			_presenter = new PresenterBuilder(_serviceProvider, new GameObject("PresentContextTests")).UseViewFactory(_viewFactory).Build();
		}

		[TearDown]
		public void Dispose()
		{
			_presenter.Dispose();
		}

		[Test]
		public void Tag_IsValid()
		{
			var controller = _presenter.Present<TagController>().Controller;
			Assert.AreEqual(TagController.TagValue, controller.Tag);
		}
	}
}
