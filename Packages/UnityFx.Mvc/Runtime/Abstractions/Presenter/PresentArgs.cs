// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Runtime arguments for <see cref="IPresenter"/> methods.
	/// </summary>
	public class PresentArgs
	{
		#region data

		private Dictionary<string, string> _query;

		#endregion

		#region interface

		/// <summary>
		/// Gets query parameters (if any).
		/// </summary>
		public IDictionary<string, string> Query
		{
			get
			{
				if (_query is null)
				{
					_query = new Dictionary<string, string>();
				}

				return _query;
			}
		}

		/// <summary>
		/// Gets or sets fragment parameters (if any).
		/// </summary>
		public string Fragment { get; set; }

		/// <summary>
		/// Gets or sets the controller present options.
		/// </summary>
		public PresentOptions PresentOptions { get; set; }

		/// <summary>
		/// Gets or sets a transform to attach view to (if set).
		/// </summary>
		public Transform Transform { get; set; }

		/// <summary>
		/// Gets or sets a user-defined  data.
		/// </summary>
		public object UserData { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		public PresentArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentArgs"/> class.
		/// </summary>
		public PresentArgs(object userData)
		{
			UserData = userData;
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
			var result = new PresentArgs()
			{
				Fragment = deeplink.Fragment
			};

			if (!string.IsNullOrEmpty(query))
			{
				var args = query.Split('?');
				var queryMap = result.Query;

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
			}

			return result;
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			var queryNotEmpty = _query.Count > 0;
			var fragmentNotEmpty = !string.IsNullOrEmpty(Fragment);

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
					text.Append(Fragment);
				}

				return text.ToString();
			}

			return base.ToString();
		}

		#endregion

		#region implementation
		#endregion
	}
}
