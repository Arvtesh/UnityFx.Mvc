// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityFx.Mvc
{
	partial class Presenter
	{
#if ENABLE_IL2CPP

		[Preserve]
		private static void AotCodegenHelper()
		{
			// NOTE: This method is needed for AOT compiler to generate code for PresentResult<,> specializations.
			// It should never be executed, it's just here to mark specific type arguments as used.
			new PresentResult<ViewController, object>(null, null, typeof(ViewController), PresentOptions.None, PresentArgs.Default);
			new PresentResult<ViewController, int>(null, null, typeof(ViewController), PresentOptions.None, PresentArgs.Default);
		}

#endif
	}
}
