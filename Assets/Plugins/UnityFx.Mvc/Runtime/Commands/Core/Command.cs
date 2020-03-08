// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic packed command.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct Command : IEquatable<Command>
	{
		#region data

#if UNITY_64
		private const int _ptrSize = 8;
#else
		private const int _ptrSize = 4;
#endif

		[FieldOffset(0)]
		private readonly Type _commandType;
		[FieldOffset(_ptrSize)]
		private readonly int _commandId;
		[FieldOffset(_ptrSize)]
		private readonly string _commandName;

		#endregion

		#region interface

		public Command(Type commandType, int commandId)
			: this()
		{
			_commandType = commandType;
			_commandId = commandId;
		}

		public Command(string commandName)
			: this()
		{
			_commandType = typeof(string);
			_commandName = commandName;
		}

		public unsafe TCommand ToEnum<TCommand>() where TCommand : unmanaged, Enum
		{
			fixed (int* p = &_commandId)
			{
				return *(TCommand*)p;
			}
		}

		public static Command FromEnum<TCommand>(TCommand cmd) where TCommand : struct, Enum
		{
			return new Command(typeof(TCommand), cmd.GetHashCode());
		}

		public static Command FromString(string s)
		{
			return new Command(s);
		}

		#endregion

		#region IEquatable

		public bool Equals(Command other)
		{
			if (_commandType != other._commandType)
			{
				return false;
			}

			if (_commandType == typeof(string))
			{
				return string.CompareOrdinal(_commandName, other._commandName) == 0;
			}

			return _commandId == other._commandId;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			if (_commandType is null)
			{
				return "null";
			}

			if (_commandType == typeof(string))
			{
				return _commandName;
			}

			if (_commandType.IsEnum)
			{
				return Enum.GetName(_commandType, _commandId);
			}

			if (_commandType == typeof(int))
			{
				return _commandId.ToString();
			}

			return $"{_commandType.Name}({_commandId})";
		}

		#endregion
	}
}
