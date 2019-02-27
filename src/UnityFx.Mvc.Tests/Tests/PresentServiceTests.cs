// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;

namespace UnityFx.Mvc
{
	public class PresentServiceTests
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IPresentService _mvcService;

		public PresentServiceTests()
		{
			_serviceProvider = new FakeServiceProvider();
			_mvcService = new PresentService(_serviceProvider);
		}
	}
}
