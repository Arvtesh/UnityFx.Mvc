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
	[Category("ViewFactory"), TestOf(typeof(UGUIViewFactory))]
	public class ViewFactoryTests : IDisposable
	{
		private GameObject _go;
		private UGUIViewFactory _viewFactory;

		[SetUp]
		public void Init()
		{
			_go = new GameObject("ViewFactoryTest");
			_viewFactory = _go.AddComponent<UGUIViewFactory>();
		}

		[TearDown]
		public void Dispose()
		{
			_viewFactory.Dispose();
		}

		[Test]
		public void InitialStateIsValid()
		{
			Assert.NotNull(_viewFactory.Views);
			Assert.IsEmpty(_viewFactory.Views);
		}
	}
}
