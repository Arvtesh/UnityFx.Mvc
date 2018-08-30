// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Represents an error that occurs when a service is not registered.
	/// </summary>
	public class ServiceNotFoundException : Exception
	{
		/// <summary>
		/// Gets service type.
		/// </summary>
		public Type ServiceType { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
		/// </summary>
		public ServiceNotFoundException(Type serviceType)
			: base(string.Format("Service {0} is not registered.", serviceType.Name))
		{
			ServiceType = serviceType;
		}
	}
}
