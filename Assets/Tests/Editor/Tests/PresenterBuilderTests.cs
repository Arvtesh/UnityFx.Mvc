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
	[Category("PresenterBuilder"), TestOf(typeof(PresenterBuilder))]
	public class PresenterBuilderTests : IDisposable
	{
		private DefaultServiceProvider _serviceProvider;
		private GameObject _go;
		private PresenterBuilder _builder;

		[SetUp]
		public void Init()
		{
			_serviceProvider = new DefaultServiceProvider();
			_go = new GameObject("PresenterBuilderTests");
			_builder = new PresenterBuilder(_serviceProvider, _go);
		}

		[TearDown]
		public void Dispose()
		{
			GameObject.DestroyImmediate(_go);
		}

		[Test]
		public void Build_ThrowsIfViewFactoryIsNotSet()
		{
			Assert.Throws<InvalidOperationException>(() => _builder.Build());
		}

		[Test]
		public void Build_SucceedsIfViewFactoryIsSet()
		{
			var presenter = _builder.UseViewFactory(new DefaultViewFactory()).Build();

			Assert.NotNull(presenter);
		}
	}
}
