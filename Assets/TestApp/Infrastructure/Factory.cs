using System;
using UnityEngine;

namespace TestApp.Infrastructure
{
	public static class Factory
	{
		public static IWebApi CreateWebApi(GameObject serviceRoot)
		{
			return serviceRoot.AddComponent<DummyWebApi>();
		}
	}
}
