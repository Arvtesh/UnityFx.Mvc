// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IAppView"/>.
	/// </summary>
	internal class AppView : DisposableMonoBehaviour, IAppView, ICollection<GameObject>
	{
		#region data

		private bool _enabled = true;
		private bool _interactable = true;
		private bool _exclusiveParent;
		private bool _exclusive;

		#endregion

		#region interface
		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
			SetDisposed();
		}

		#endregion

		#region IAppView

		public string Name
		{
			get
			{
				ThrowIfDisposed();
				return name;
			}
		}

		public ICollection<GameObject> Content
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		public Bounds Bounds
		{
			get
			{
				ThrowIfDisposed();

				var result = new Bounds(transform.position, Vector3.zero);
				var childCount = transform.childCount;

				if (childCount > 0)
				{
					var renderers = new List<Renderer>(childCount);

					for (var i = 0; i < childCount; ++i)
					{
						transform.GetChild(i).GetComponentsInChildren(true, renderers);
					}

					foreach (var renderer in renderers)
					{
						result.Encapsulate(renderer.bounds);
					}
				}

				return result;
			}
		}

		public bool IsExclusive => _exclusive;

		public bool IsEnabled => _enabled;

		public bool IsInteractable => _interactable;

		public void SetEnabled(bool enabled)
		{
			ThrowIfDisposed();

			_enabled = enabled;
			gameObject.SetActive(enabled && !_exclusiveParent);
		}

		public void SetExclusive(bool exclusive)
		{
			ThrowIfDisposed();

			if (_exclusive != exclusive && !ReferenceEquals(transform.parent, null))
			{
				for (var i = transform.GetSiblingIndex() - 1; i >= 0; --i)
				{
					var view = transform.parent.GetChild(i).GetComponent<AppView>();

					if (!ReferenceEquals(view, null))
					{
						exclusive = SetParentExclusive(exclusive);
					}
				}

				_exclusive = exclusive;
			}
		}

		public void SetInteractable(bool inputEnabled)
		{
			ThrowIfDisposed();

			var childCount = transform.childCount;

			if (childCount > 0)
			{
				var grs = new List<GraphicRaycaster>(childCount);

				for (var i = 0; i < childCount; ++i)
				{
					transform.GetChild(i).GetComponentsInChildren(true, grs);
				}

				foreach (var gr in grs)
				{
					gr.enabled = inputEnabled;
				}
			}

			_interactable = inputEnabled;
		}

		#endregion

		#region ICollection

		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return transform.childCount;
			}
		}

		public bool IsReadOnly => false;

		public void Add(GameObject go)
		{
			ThrowIfDisposed();

			if (ReferenceEquals(go, null))
			{
				throw new ArgumentNullException(nameof(go));
			}

			go.transform.SetParent(transform, false);
		}

		public bool Remove(GameObject go)
		{
			ThrowIfDisposed();

			if (!ReferenceEquals(go, null))
			{
				for (var i = 0; i < transform.childCount; ++i)
				{
					var t = transform.GetChild(i);

					if (ReferenceEquals(go.transform, t))
					{
						Destroy(t.gameObject);
						return true;
					}
				}
			}

			return false;
		}

		public bool Contains(GameObject go)
		{
			ThrowIfDisposed();

			if (!ReferenceEquals(go, null))
			{
				for (var i = 0; i < transform.childCount; ++i)
				{
					if (ReferenceEquals(transform, transform.GetChild(i)))
					{
						return true;
					}
				}
			}

			return false;
		}

		public void CopyTo(GameObject[] array, int arrayIndex)
		{
			ThrowIfDisposed();

			if (array != null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			for (var i = 0; i < transform.childCount; ++i)
			{
				array[i + arrayIndex] = transform.GetChild(i).gameObject;
			}
		}

		public void Clear()
		{
			ThrowIfDisposed();

			while (transform.childCount > 0)
			{
				Destroy(transform.GetChild(0).gameObject);
			}
		}

		#endregion

		#region IEnumerable

		public IEnumerator<GameObject> GetEnumerator() => GetEnumeratorInternal();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

		#endregion

		#region implementation

		private bool SetParentExclusive(bool parentExclusive)
		{
			_exclusiveParent = parentExclusive;
			gameObject.SetActive(_enabled && !parentExclusive);
			return parentExclusive || _exclusive;
		}

		private IEnumerator<GameObject> GetEnumeratorInternal()
		{
			ThrowIfDisposed();

			for (var i = 0; i < transform.childCount; ++i)
			{
				yield return transform.GetChild(i).gameObject;
			}
		}

		#endregion
	}
}
