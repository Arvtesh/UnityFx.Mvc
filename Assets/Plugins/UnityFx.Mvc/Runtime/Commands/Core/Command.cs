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
		private readonly object _command;

		#endregion

		#region interface

		public bool IsNull => _commandType is null;

		public bool IsEnum => _commandType != null && _commandType.IsEnum;

		public bool IsString => _commandType != null && _commandType == typeof(string);

		public bool IsInt => _commandType != null && _commandType == typeof(int);

		public Type Type => _commandType;

		public Command(Type commandType, int commandId)
			: this()
		{
			_commandType = commandType;
			_commandId = commandId;
		}

		public Command(int commandId)
			: this()
		{
			_commandType = typeof(int);
			_commandId = commandId;
		}

		public Command(object cmd)
			: this()
		{
			if (cmd != null)
			{
				_commandType = cmd.GetType();
				_command = cmd;
			}
		}

		public unsafe bool TryUnpackEnum<T>(out T value) where T : unmanaged, Enum
		{
			if (_commandType == typeof(T))
			{
				fixed (int* p = &_commandId)
				{
					value = *(T*)p;
					return true;
				}
			}

			value = default;
			return false;
		}

		public bool TryUnpackString(out string value)
		{
			if (_commandType == typeof(string))
			{
				value = (string)_command;
				return true;
			}

			value = default;
			return false;
		}

		public bool TryUnpackInt(out int value)
		{
			if (_commandType == typeof(int))
			{
				value = _commandId;
				return true;
			}

			value = default;
			return false;
		}

		public int ToInt()
		{
			if (TryUnpackInt(out var result))
			{
				return result;
			}

			throw new InvalidCastException();
		}

		public TCommand ToEnum<TCommand>() where TCommand : unmanaged, Enum
		{
			if (TryUnpackEnum(out TCommand result))
			{
				return result;
			}

			throw new InvalidCastException();
		}

		public static Command FromType<TCommand>(TCommand cmd)
		{
			if (typeof(TCommand).IsEnum)
			{
				return new Command(typeof(TCommand), cmd.GetHashCode());
			}
			else if (typeof(TCommand) == typeof(int))
			{
				return new Command(cmd.GetHashCode());
			}

			return new Command(cmd);
		}

		public static Command FromEnum<TCommand>(TCommand cmd) where TCommand : struct, Enum
		{
			return new Command(typeof(TCommand), cmd.GetHashCode());
		}

		public static Command FromString(string s)
		{
			return new Command(s);
		}

		public static Command FromInt(int n)
		{
			return new Command(n);
		}

		/// <summary>
		/// Implicit convertion from <see cref="int"/>.
		/// </summary>
		public static implicit operator Command(int value) => new Command(value);

		/// <summary>
		/// Implicit convertion from <see cref="string"/>.
		/// </summary>
		public static implicit operator Command(string value) => new Command(value);

		#endregion

		#region IEquatable

		public bool Equals(Command other)
		{
			if (_commandType != other._commandType || _commandType is null)
			{
				return false;
			}

			if (_commandType == typeof(int) || _commandType.IsEnum)
			{
				return _commandId == other._commandId;
			}

			return _command.Equals(other._command);
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
				return _command.ToString();
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
