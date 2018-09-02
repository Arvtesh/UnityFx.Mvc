// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Extensions for <see cref="IServiceProvider"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static class ServiceProviderExtensions
	{
		/// <summary>
		/// Returns an instance of a service for the type specified.
		/// </summary>
		/// <typeparam name="TService">Type of the requested service.</typeparam>
		/// <param name="serviceProvider">A service provider.</param>
		/// <returns>Returns service instance registered for the <typeparamref name="TService"/> type.</returns>
		public static TService GetService<TService>(this IServiceProvider serviceProvider)
		{
			return (TService)serviceProvider.GetService(typeof(TService));
		}
	}
}
