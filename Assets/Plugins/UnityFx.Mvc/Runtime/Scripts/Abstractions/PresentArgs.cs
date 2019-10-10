// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Arguments for <see cref="IPresenter"/> methods.
	/// </summary>
	public class PresentArgs
	{
		#region data

		private static Dictionary<string, string> _emptyQuery = new Dictionary<string, string>();
		private static PresentArgs _defaultArgs = new PresentArgs();

		private readonly Dictionary<string, string> _query;
		private readonly string _fragment;

		#endregion

		#region interface

		/// <summary>
		/// Gets default arguments value.
		/// </summary>
		public static PresentArgs Default => _defaultArgs;

		/// <summary>
		/// Gets query parameters (if any).
		/// </summary>
		public IReadOnlyDictionary<string, string> Query => _query;

		/// <summary>
		/// Gets fragment parameters (if any).
		/// </summary>
		public string Fragment => _fragment;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		public PresentArgs()
		{
			_query = _emptyQuery;
			_fragment = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="query"></param>
		public PresentArgs(IEnumerable<KeyValuePair<string, string>> query)
			: this(query, string.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="query"></param>
		/// <param name="fragment"></param>
		public PresentArgs(IEnumerable<KeyValuePair<string, string>> query, string fragment)
		{
			if (query is null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			var data = new Dictionary<string, string>();

			foreach (var item in query)
			{
				data.Add(item.Key, item.Value);
			}

			_query = data;
			_fragment = fragment ?? string.Empty;
		}

		/// <summary>
		/// Creates a <see cref="PresentArgs"/> instance from a <paramref name="deeplink"/> specified.
		/// </summary>
		/// <param name="deeplink">A deeplink.</param>
		/// <returns>A <see cref="PresentArgs"/> instance representing the specified deeplink.</returns>
		public static PresentArgs FromDeeplink(Uri deeplink)
		{
			if (deeplink is null)
			{
				throw new ArgumentNullException(nameof(deeplink));
			}

			var query = deeplink.Query;
			var fragment = deeplink.Fragment;

			if (string.IsNullOrEmpty(query))
			{
				return new PresentArgs(deeplink, _emptyQuery, fragment);
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

				return new PresentArgs(deeplink, queryMap, fragment);
			}
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
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

		#endregion

		#region implementation

		private PresentArgs(Uri deeplink, Dictionary<string, string> query, string fragment)
		{
			_query = query;
			_fragment = fragment ?? string.Empty;
		}

		#endregion
	}
}
