// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Arguments for <see cref="IAppStateManager.PushStateAsync(Type, PushStateArgs)"/>.
	/// </summary>
	public class PushStateArgs
	{
		#region data

		private static Dictionary<string, string> _emptyQuery = new Dictionary<string, string>();
		private static PushStateArgs _defaultArgs;

		private readonly Uri _deeplink;
		private readonly PushOptions _options;
		private readonly object _data;

		private Dictionary<string, string> _query;
		private string _fragment;

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
					_defaultArgs = new PushStateArgs(PushOptions.Push);
				}

				return _defaultArgs;
			}
		}

		/// <summary>
		/// Gets state creation options.
		/// </summary>
		public PushOptions Options => _options;

		/// <summary>
		/// Get user data attached to this object.
		/// </summary>
		public object Data => _data;

		/// <summary>
		/// Gets the deeplink this state was created with or <see langword="null"/> if no deeplink was specified.
		/// </summary>
		public Uri Deeplink => _deeplink;

		/// <summary>
		/// Gets query parameters (if any).
		/// </summary>
#if NET35
		public IDictionary<string, string> Query
#else
		public IReadOnlyDictionary<string, string> Query
#endif
		{
			get
			{
				if (_query == null)
				{
					if (_deeplink == null)
					{
						_query = _emptyQuery;
					}
					else
					{
						var query = _deeplink.Query;

						if (string.IsNullOrEmpty(query))
						{
							_query = _emptyQuery;
						}
						else
						{
							var args = query.Split('?');
							var queryParams = new Dictionary<string, string>(args.Length);

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

								if (!string.IsNullOrEmpty(key) && !queryParams.ContainsKey(key))
								{
									queryParams.Add(key, value);
								}
							}

							_query = queryParams;
						}
					}
				}

				return _query;
			}
		}

		/// <summary>
		/// Gets fragment parameters (if any).
		/// </summary>
		public string Fragment
		{
			get
			{
				if (_fragment == null)
				{
					if (_deeplink == null)
					{
						_fragment = string.Empty;
					}
					else
					{
						_fragment = _deeplink.Fragment;
					}
				}

				return _fragment;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		public PushStateArgs(Uri deeplink)
		{
			_deeplink = deeplink;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="options"></param>
		public PushStateArgs(PushOptions options)
		{
			_options = options;
			_query = _emptyQuery;
			_fragment = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userData"></param>
		public PushStateArgs(PushOptions options, object userData)
		{
			_options = options;
			_data = userData;
			_query = _emptyQuery;
			_fragment = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userData"></param>
		/// <param name="queryParams"></param>
		public PushStateArgs(PushOptions options, object userData, IEnumerable<KeyValuePair<string, string>> queryParams)
			: this(options, userData, queryParams, string.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateArgs"/> class.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userData"></param>
		/// <param name="queryParams"></param>
		/// <param name="fragmentParams"></param>
		public PushStateArgs(PushOptions options, object userData, IEnumerable<KeyValuePair<string, string>> queryParams, string fragmentParams)
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

			_options = options;
			_data = userData;
			_query = data;
			_fragment = fragmentParams ?? string.Empty;
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			if (_deeplink != null)
			{
				return _deeplink.ToString();
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
