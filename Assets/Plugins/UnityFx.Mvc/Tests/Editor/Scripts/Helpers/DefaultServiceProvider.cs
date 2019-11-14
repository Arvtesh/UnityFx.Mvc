// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class DefaultServiceProvider : IServiceProvider
	{
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IServiceProvider))
			{
				return this;
			}

			return null;
		}
	}
}
