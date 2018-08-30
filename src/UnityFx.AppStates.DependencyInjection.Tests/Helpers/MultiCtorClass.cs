// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates.DependencyInjection
{
	internal class MultiCtorClass
	{
		public MultiCtorClass(int arg)
		{
		}

		public MultiCtorClass(SelfReferenceClass invalidReference)
		{
		}

		public MultiCtorClass()
		{
		}
	}
}
