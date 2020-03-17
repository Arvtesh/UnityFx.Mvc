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

		/// <summary>
		/// Gets a value indicating whether the instance is an null command.
		/// </summary>
		public bool IsNull => _commandType is null;

		/// <summary>
		/// Gets a value indicating whether the command instance wraps an enumeration.
		/// </summary>
		public bool IsEnum => _commandType != null && _commandType.IsEnum;

		/// <summary>
		/// Gets a value indicating whether the command instance wraps a string.
		/// </summary>
		public bool IsString => _commandType != null && _commandType == typeof(string);

		/// <summary>
		/// Gets a value indicating whether the command instance wraps an integer.
		/// </summary>
		public bool IsInt => _commandType != null && _commandType == typeof(int);

		/// <summary>
		/// Gets the command underlying type.
		/// </summary>
		public Type Type => _commandType;

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> struct.
		/// </summary>
		private Command(Type commandType, int commandId)
			: this()
		{
			_commandType = commandType;
			_commandId = commandId;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> struct.
		/// </summary>
		private Command(Type commandType, object cmd)
			: this()
		{
			_commandType = commandType;
			_command = cmd;
		}

		/// <summary>
		/// Attempts to unpack the underlying enumeration value.
		/// </summary>
		public unsafe bool TryUnpackEnum<T>(out T value) where T : unmanaged, Enum
		{
			// TODO: Use .netstandard 2.1 unsafe helpers when available in Unity.
			if (_commandType == typeof(T) && sizeof(T) == sizeof(int))
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

		/// <summary>
		/// Attempts to unpack the underlying string value.
		/// </summary>
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

		/// <summary>
		/// Attempts to unpack the underlying integer value.
		/// </summary>
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

		/// <summary>
		/// Unpacks the underlying command.
		/// </summary>
		public object ToObject()
		{
			if (_commandType is null)
			{
				return null;
			}

			if (_commandType.IsEnum)
			{
				return Enum.ToObject(_commandType, _commandId);
			}

			if (_commandType.IsPrimitive)
			{
				if (_commandType == typeof(int))
				{
					return _commandId;
				}
			}

			return _command;
		}

		/// <summary>
		/// Unpacks the underlying integer value.
		/// </summary>
		public int ToInt()
		{
			if (_commandType is null)
			{
				return 0;
			}

			if (_commandType.IsEnum || _commandType == typeof(int))
			{
				return _commandId;
			}

			return default;
		}

		/// <summary>
		/// Unpacks the underlying enumeration value.
		/// </summary>
		public TCommand ToEnum<TCommand>() where TCommand : unmanaged, Enum
		{
			if (TryUnpackEnum(out TCommand result))
			{
				return result;
			}

			return default;
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from a generic type.
		/// </summary>
		public static Command FromType<TCommand>(TCommand cmd)
		{
			if (typeof(TCommand).IsEnum)
			{
				return new Command(typeof(TCommand), cmd.GetHashCode());
			}
			else if (typeof(TCommand) == typeof(int))
			{
				return new Command(typeof(int), cmd.GetHashCode());
			}

			return new Command(typeof(TCommand), cmd);
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from an enumeration.
		/// </summary>
		public static Command FromEnum<TCommand>(TCommand cmd) where TCommand : struct, Enum
		{
			return new Command(typeof(TCommand), cmd.GetHashCode());
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from string.
		/// </summary>
		public static Command FromString(string s)
		{
			return new Command(typeof(string), s);
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from integer.
		/// </summary>
		public static Command FromInt(int n)
		{
			return new Command(typeof(int), n);
		}

		/// <summary>
		/// Implicit convertion from <see cref="int"/>.
		/// </summary>
		public static implicit operator Command(int value) => new Command(typeof(int), value);

		/// <summary>
		/// Implicit convertion from <see cref="string"/>.
		/// </summary>
		public static implicit operator Command(string value) => new Command(typeof(string), value);

		#endregion

		#region IEquatable

		/// <inheritdoc/>
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

			if (_command != null)
			{
				return _command.Equals(other._command);
			}

			return other._command == null;
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			if (_commandType is null)
			{
				return "null";
			}

			if (_commandType == typeof(string))
			{
				return _command?.ToString() ?? string.Empty;
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
