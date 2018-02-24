// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
#if NET35

	/// <summary>
	/// Represents a generic read-only collection of key/value pairs.
	/// </summary>
	public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
		/// <summary>
		/// Gets the element that has the specified key in the read-only dictionary.
		/// </summary>
		/// <param name="key">The key to locate.</param>
		/// <returns>The element that has the specified key in the read-only dictionary.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
		/// <exception cref="KeyNotFoundException">The property is retrieved and key is not found.</exception>
		TValue this[TKey key] { get; }

		/// <summary>
		/// Gets an enumerable collection that contains the keys in the read-only dictionary.
		/// </summary>
		/// <value>An enumerable collection that contains the keys in the read-only dictionary.</value>
		/// <seealso cref="Values"/>
		IEnumerable<TKey> Keys { get; }

		/// <summary>
		/// Gets an enumerable collection that contains the values in the read-only dictionary.
		/// </summary>
		/// <value>An enumerable collection that contains the values in the read-only dictionary.</value>
		/// <seealso cref="Keys"/>
		IEnumerable<TValue> Values { get; }

		/// <summary>
		/// Determines whether the read-only dictionary contains an element that has the specified key.
		/// </summary>
		/// <param name="key">The key to locate.</param>
		/// <returns>Returns <see langword="true"/> if the read-only dictionary contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
		/// <seealso cref="TryGetValue(TKey, out TValue)"/>
		bool ContainsKey(TKey key);

		/// <summary>
		/// Gets the value that is associated with the specified key.
		/// </summary>
		/// <param name="key">The key to locate.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise,
		/// the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
		/// <returns>Returns <see langword="true"/> if the object that implements the <see cref="IReadOnlyDictionary{TKey, TValue}"/> interface
		/// contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
		/// <seealso cref="this[TKey]"/>
		/// <seealso cref="ContainsKey(TKey)"/>
		bool TryGetValue(TKey key, out TValue value);
    }

#endif
}
