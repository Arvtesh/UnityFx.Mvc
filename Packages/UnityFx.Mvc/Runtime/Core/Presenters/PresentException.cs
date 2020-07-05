// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic present error.
	/// </summary>
	public class PresentException : Exception
	{
		#region data

		private const string _controllerTypeName = "_assetName";

		private readonly Type _controllerType;

		#endregion

		#region interface

		/// <summary>
		/// Gets the controller info.
		/// </summary>
		public Type ControllerType => _controllerType;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentException"/> class.
		/// </summary>
		public PresentException(Type controllerType)
			: base(GetMessage(controllerType, null))
		{
			_controllerType = controllerType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentException"/> class.
		/// </summary>
		public PresentException(Type controllerType, string message)
			: base(GetMessage(controllerType, message))
		{
			_controllerType = controllerType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentException"/> class.
		/// </summary>
		public PresentException(Type controllerType, Exception innerException)
			: base(GetMessage(controllerType, null), innerException)
		{
			_controllerType = controllerType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentException"/> class.
		/// </summary>
		public PresentException(IPresentInfo presentInfo)
			: base(GetMessage(presentInfo, null))
		{
			_controllerType = presentInfo.ControllerType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentException"/> class.
		/// </summary>
		public PresentException(IPresentInfo controllerInfo, string message)
			: base(GetMessage(controllerInfo, message))
		{
			_controllerType = controllerInfo.ControllerType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentException"/> class.
		/// </summary>
		public PresentException(IPresentInfo controllerInfo, Exception innerException)
			: base(GetMessage(controllerInfo, null), innerException)
		{
			_controllerType = controllerInfo.ControllerType;
		}

		#endregion

		#region ISerializable

		private PresentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_controllerType = (Type)info.GetValue(_controllerTypeName, typeof(Type));

		}

		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(_controllerTypeName, _controllerType);
		}

		#endregion

		#region implementation

		private static string GetMessage(Type controllerType, string message)
		{
			if (controllerType is null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (string.IsNullOrEmpty(message))
			{
				return $"Failed to present controller {controllerType.Name}.";
			}
			else
			{
				var s = $"Failed to present controller {controllerType.Name}: {message}";

				if (s[s.Length - 1] != '.')
				{
					s += '.';
				}

				return s;
			}
		}

		private static string GetMessage(IPresentInfo controllerInfo, string message)
		{
			if (controllerInfo is null)
			{
				throw new ArgumentNullException(nameof(controllerInfo));
			}

			if (string.IsNullOrEmpty(message))
			{
				return $"Failed to present controller {controllerInfo.ControllerType.Name}.";
			}
			else
			{
				return $"Failed to present controller {controllerInfo.ControllerType.Name}: {message}.";
			}
		}

		#endregion

	}
}
