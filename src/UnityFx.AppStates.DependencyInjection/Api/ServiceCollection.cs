// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Default implementation of <see cref="IServiceCollection"/>.
	/// </summary>
	public class ServiceCollection : IServiceCollection
	{
		#region data

		private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();

		#endregion

		#region interface
		#endregion

		#region IServiceCollection

		/// <inheritdoc/>
		public int Count => _services.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		/// <inheritdoc/>
		public void Add(ServiceDescriptor item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			_services.Add(item.ServiceType, item);
		}

		/// <inheritdoc/>
		public bool Remove(ServiceDescriptor item)
		{
			if (item != null)
			{
				return _services.Remove(item.ServiceType);
			}

			return false;
		}

		/// <inheritdoc/>
		public bool Contains(ServiceDescriptor item)
		{
			if (item != null)
			{
				return _services.ContainsKey(item.ServiceType);
			}

			return false;
		}

		/// <inheritdoc/>
		public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
		{
			_services.Values.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		public void Clear()
		{
			_services.Clear();
		}

		#endregion

		#region IEnumerable

		/// <inheritdoc/>
		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return _services.Values.GetEnumerator();
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _services.Values.GetEnumerator();
		}

		#endregion

		#region implementation
		#endregion
	}
}
