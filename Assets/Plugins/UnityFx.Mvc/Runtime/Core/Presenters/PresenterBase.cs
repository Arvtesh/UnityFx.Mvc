// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Abstract presenter. This is needed for Unity inspector.
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IViewController"/>
	public abstract class PresenterBase : MonoBehaviour
	{
		/// <summary>
		/// Gets all controllers presented.
		/// </summary>
		internal abstract IReadOnlyCollection<IViewController> GetControllers();
	}
}
