// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Arguments for <see cref="IAppStateService.PushStateAsync(Type, PushStateArgs)"/>.
	/// </summary>
	public class PushStateArgs
	{
		#region data

		private static Dictionary<string, string> _emptyQuery = new Dictionary<string, string>();
		private static PushStateArgs _defaultArgs;

		private readonly object _data;
		private readonly Dictionary<string, string> _query;
		private readonly string _fragment;

		#endregion

		#region interface

		/// <summary>
		/// Gets default arguments value.
		/// </summary>
		public static PushStateArgs Default
		{
			get
			{
				if (_defaultArgs == null)
				{
					_defaultArgs = new PushStateArgs();
				}

				return _defaultArgs;
			}
		}

		/// <summary>
		/// Creates a <see cref="PushStateArgs"/> instance from a <paramref name="deeplink"/> specified.
		/// </summary>
		/// <param name="deeplink">A deeplink.</param>
		/// <returns>A <see cref="PushStateArgs"/> instance representing the specified deeplink.</returns>
		public static PushStateArgs FromDeeplink(Uri deeplink)
		{
			if (deeplink == null)
			{
				throw new ArgumentNullException(nameof(deeplink));
			}

			var query = deeplink.Query;
			var fragment = deeplink.Fragment;

			if (string.IsNullOrEmpty(query))
			{
				return new PushStateArgs(deeplink, _emptyQuery, fragment);
			}
			else
			{
				var args = query.Split('?');
				var queryMap = new Dictionary<string, string>(args.Length);

				foreach (var arg in args)
				{
					var index = arg.IndexOf('=');
					var key = arg;
					var value = string.Empty;

					if (index >= 0)
					{
						key = arg.Substring(0, index);
						value = arg.Substring(index);
					}

					if (!string.IsNullOrEmpty(key) && !queryMap.ContainsKey(key))
					{
						queryMap.Add(key, value);
					}
				}

				return new PushStateArgs(deeplink, queryMap, fragment);
			}
		}

		/// <summary>
		/// Gets user data attached to this object.
		/// </summary>
		public object Data => _data;

		/// <summary>
		/// Gets query parameters (if any).
		/// </summary>
#if NET35
		public IDictionary<string, string> Query => _query;
#else
		public IReadOnlyDictionary<string, string> Query => _query;
#endif

		/// <summary>
		/// Gets fragment parameters (if any).
		/// </summary>
		public string Fragment => _fragment;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		public PushStateArgs()
		{
			_query = _emptyQuery;
			_fragment = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="userData"></param>
		public PushStateArgs(object userData)
		{
			_data = userData;
			_query = _emptyQuery;
			_fragment = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="queryParams"></param>
		public PushStateArgs(IEnumerable<KeyValuePair<string, string>> queryParams, object userData)
			: this(queryParams, string.Empty, userData)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="queryParams"></param>
		/// <param name="fragmentParams"></param>
		public PushStateArgs(IEnumerable<KeyValuePair<string, string>> queryParams, string fragmentParams, object userData)
		{
			if (queryParams == null)
			{
				throw new ArgumentNullException(nameof(queryParams));
			}

			var data = new Dictionary<string, string>();

			foreach (var item in queryParams)
			{
				data.Add(item.Key, item.Value);
			}

			_data = userData;
			_query = data;
			_fragment = fragmentParams ?? string.Empty;
		}

		private PushStateArgs(Uri deeplink, Dictionary<string, string> query, string fragment)
		{
			_data = deeplink;
			_query = query;
			_fragment = fragment ?? string.Empty;
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			if (_data is Uri deeplink)
			{
				return deeplink.ToString();
			}
			else
			{
				var queryNotEmpty = _query.Count > 0;
				var fragmentNotEmpty = !string.IsNullOrEmpty(_fragment);

				if (queryNotEmpty || fragmentNotEmpty)
				{
					var text = new StringBuilder();

					if (queryNotEmpty)
					{
						var first = true;

						foreach (var item in _query)
						{
							if (first)
							{
								first = false;
							}
							else
							{
								text.Append('?');
							}

							text.Append(item.Key);

							if (!string.IsNullOrEmpty(item.Value))
							{
								text.Append(item.Value);
							}
						}
					}

					if (fragmentNotEmpty)
					{
						text.Append('#');
						text.Append(_fragment);
					}

					return text.ToString();
				}

				return base.ToString();
			}
		}

		#endregion
	}
}
