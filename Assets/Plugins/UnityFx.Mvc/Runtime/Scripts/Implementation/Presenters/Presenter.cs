// Copyright (c) Alexander Bogarsukov.
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
	/// Default <see cref="MonoBehaviour"/>-based presenter implementation.
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IViewController"/>
	public class Presenter : Presenter<IViewController>
	{
	}
}
