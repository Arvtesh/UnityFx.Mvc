// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class DummyView : AppView
	{
		public DummyView(string id, AppViewOptions options) : base(id, options) {}
		public DummyView(string id, AppView parent, AppViewOptions options) : base(id, parent, options) { }
		public override TComponent GetComponent<TComponent>() => default(TComponent);
		public override TComponent GetComponentRecursive<TComponent>() => default(TComponent);
		public override TComponent[] GetComponents<TComponent>() => null;
		public override TComponent[] GetComponentsRecursive<TComponent>() => null;
		protected override IAsyncOperation LoadContent(string id) => AsyncResult.Delay(1);
		protected override void SetEnabled(bool visible) { }
		protected override void SetVisible(bool visible) { }
	}
}
