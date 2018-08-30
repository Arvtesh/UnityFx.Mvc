// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Represents an error that occurs when a service defines usable no constructors.
	/// </summary>
	public class ServiceConstructorResolutionException : Exception
	{
		/// <summary>
		/// Gets service type.
		/// </summary>
		public Type ServiceType { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceConstructorResolutionException"/> class.
		/// </summary>
		public ServiceConstructorResolutionException(Type serviceType)
			: base(string.Format("None of {0} constructors can be used to create a service instance.", serviceType.Name))
		{
			ServiceType = serviceType;
		}
	}
}
