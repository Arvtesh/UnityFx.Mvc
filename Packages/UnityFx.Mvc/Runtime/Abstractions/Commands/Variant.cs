// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A variant data type.
	/// </summary>
	/// <seealso cref="Command"/>
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct Variant : IEquatable<Variant>, IComparable<Variant>, IConvertible, IFormattable
	{
		#region data

#if UNITY_64
		private const int _ptrSize = 8;
#else
		private const int _ptrSize = 4;
#endif

		[FieldOffset(0)]
		private readonly TypeCode _typeCode;
		[FieldOffset(_ptrSize)]
		private readonly bool _bool;
		[FieldOffset(_ptrSize)]
		private readonly char _char;
		[FieldOffset(_ptrSize)]
		private readonly byte _byte;
		[FieldOffset(_ptrSize)]
		private readonly sbyte _sbyte;
		[FieldOffset(_ptrSize)]
		private readonly short _short;
		[FieldOffset(_ptrSize)]
		private readonly ushort _ushort;
		[FieldOffset(_ptrSize)]
		private readonly int _int;
		[FieldOffset(_ptrSize)]
		private readonly uint _uint;
		[FieldOffset(_ptrSize)]
		private readonly long _long;
		[FieldOffset(_ptrSize)]
		private readonly ulong _ulong;
		[FieldOffset(_ptrSize)]
		private readonly float _float;
		[FieldOffset(_ptrSize)]
		private readonly double _double;
		[FieldOffset(_ptrSize)]
		private readonly DateTime _dateTime;
		[FieldOffset(_ptrSize)]
		private readonly string _string;
		[FieldOffset(_ptrSize)]
		private readonly object _object;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the instance is null.
		/// </summary>
		public bool IsNull => _typeCode == TypeCode.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(bool value)
			: this()
		{
			_typeCode = TypeCode.Boolean;
			_bool = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(char value)
			: this()
		{
			_typeCode = TypeCode.Char;
			_char = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(byte value)
			: this()
		{
			_typeCode = TypeCode.Byte;
			_byte = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(sbyte value)
			: this()
		{
			_typeCode = TypeCode.SByte;
			_sbyte = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(short value)
			: this()
		{
			_typeCode = TypeCode.Int16;
			_short = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(ushort value)
			: this()
		{
			_typeCode = TypeCode.UInt16;
			_ushort = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(int value)
			: this()
		{
			_typeCode = TypeCode.Int32;
			_int = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(uint value)
			: this()
		{
			_typeCode = TypeCode.UInt32;
			_uint = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(long value)
			: this()
		{
			_typeCode = TypeCode.Int64;
			_long = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(ulong value)
			: this()
		{
			_typeCode = TypeCode.UInt64;
			_ulong = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(float value)
			: this()
		{
			_typeCode = TypeCode.Single;
			_float = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(double value)
			: this()
		{
			_typeCode = TypeCode.Double;
			_double = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(DateTime value)
			: this()
		{
			_typeCode = TypeCode.DateTime;
			_dateTime = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(string value)
			: this()
		{
			_typeCode = TypeCode.String;
			_string = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Variant"/> struct.
		/// </summary>
		public Variant(object value)
			: this()
		{
			_typeCode = TypeCode.Object;
			_object = value;
		}

		/// <summary>
		/// Explicit cast to <see cref="object"/>.
		/// </summary>
		public object ToObject()
		{
			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return _bool;
				case TypeCode.Byte:
					return _byte;
				case TypeCode.Char:
					return _char;
				case TypeCode.DateTime:
					return _dateTime;
				case TypeCode.Double:
					return _double;
				case TypeCode.Int16:
					return _short;
				case TypeCode.Int32:
					return _int;
				case TypeCode.Int64:
					return _long;
				case TypeCode.Object:
					return _object;
				case TypeCode.SByte:
					return _sbyte;
				case TypeCode.Single:
					return _float;
				case TypeCode.String:
					return _string;
				case TypeCode.UInt16:
					return _ushort;
				case TypeCode.UInt32:
					return _uint;
				case TypeCode.UInt64:
					return _ulong;
			}

			return null;
		}

		/// <summary>
		/// Cast to a generic type.
		/// </summary>
		public T ToType<T>() => (T)ToType(typeof(T), CultureInfo.CurrentCulture);

		/// <summary>
		/// Cast to a generic type.
		/// </summary>
		public T ToType<T>(IFormatProvider provider) => (T)ToType(typeof(T), provider);

		/// <summary>
		/// Implicit convertion from <see cref="bool"/>.
		/// </summary>
		public static implicit operator Variant(bool value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="char"/>.
		/// </summary>
		public static implicit operator Variant(char value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="byte"/>.
		/// </summary>
		public static implicit operator Variant(byte value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="sbyte"/>.
		/// </summary>
		public static implicit operator Variant(sbyte value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="short"/>.
		/// </summary>
		public static implicit operator Variant(short value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="ushort"/>.
		/// </summary>
		public static implicit operator Variant(ushort value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="int"/>.
		/// </summary>
		public static implicit operator Variant(int value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="uint"/>.
		/// </summary>
		public static implicit operator Variant(uint value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="long"/>.
		/// </summary>
		public static implicit operator Variant(long value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="ulong"/>.
		/// </summary>
		public static implicit operator Variant(ulong value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="float"/>.
		/// </summary>
		public static implicit operator Variant(float value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="double"/>.
		/// </summary>
		public static implicit operator Variant(double value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="string"/>.
		/// </summary>
		public static implicit operator Variant(string value) => new Variant(value);

		/// <summary>
		/// Implicit convertion from <see cref="DateTime"/>.
		/// </summary>
		public static implicit operator Variant(DateTime value) => new Variant(value);

		/// <summary>
		/// Explicit <see cref="bool"/> convertion.
		/// </summary>
		public static explicit operator bool(Variant v) => v.ToBoolean(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="char"/> convertion.
		/// </summary>
		public static explicit operator char(Variant v) => v.ToChar(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="byte"/> convertion.
		/// </summary>
		public static explicit operator byte(Variant v) => v.ToByte(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="sbyte"/> convertion.
		/// </summary>
		public static explicit operator sbyte(Variant v) => v.ToSByte(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="short"/> convertion.
		/// </summary>
		public static explicit operator short(Variant v) => v.ToInt16(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="ushort"/> convertion.
		/// </summary>
		public static explicit operator ushort(Variant v) => v.ToUInt16(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="int"/> convertion.
		/// </summary>
		public static explicit operator int(Variant v) => v.ToInt32(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="uint"/> convertion.
		/// </summary>
		public static explicit operator uint(Variant v) => v.ToUInt32(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="long"/> convertion.
		/// </summary>
		public static explicit operator long(Variant v) => v.ToInt64(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="ulong"/> convertion.
		/// </summary>
		public static explicit operator ulong(Variant v) => v.ToUInt64(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="float"/> convertion.
		/// </summary>
		public static explicit operator float(Variant v) => v.ToSingle(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="double"/> convertion.
		/// </summary>
		public static explicit operator double(Variant v) => v.ToDouble(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="decimal"/> convertion.
		/// </summary>
		public static explicit operator decimal(Variant v) => v.ToDecimal(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="string"/> convertion.
		/// </summary>
		public static explicit operator string(Variant v) => v.ToString(CultureInfo.CurrentCulture);

		/// <summary>
		/// Explicit <see cref="DateTime"/> convertion.
		/// </summary>
		public static explicit operator DateTime(Variant v) => v.ToDateTime(CultureInfo.CurrentCulture);

		#endregion

		#region IEquatable

		/// <inheritdoc/>
		public bool Equals(Variant other)
		{
			return _typeCode == other._typeCode && _long == other._long;
		}

		#endregion

		#region IComparable

		/// <inheritdoc/>
		public int CompareTo(Variant other)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IFormattable

		/// <inheritdoc/>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (_typeCode == TypeCode.String)
			{
				return _string;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return _bool.ToString(formatProvider);
				case TypeCode.Byte:
					return _byte.ToString(format, formatProvider);
				case TypeCode.Char:
					return _char.ToString(formatProvider);
				case TypeCode.DateTime:
					return _dateTime.ToString(format, formatProvider);
				case TypeCode.Double:
					return _double.ToString(format, formatProvider);
				case TypeCode.Int16:
					return _short.ToString(format, formatProvider);
				case TypeCode.Int32:
					return _int.ToString(format, formatProvider);
				case TypeCode.Int64:
					return _long.ToString(format, formatProvider);
				case TypeCode.SByte:
					return _sbyte.ToString(format, formatProvider);
				case TypeCode.Single:
					return _float.ToString(format, formatProvider);
				case TypeCode.UInt16:
					return _ushort.ToString(format, formatProvider);
				case TypeCode.UInt32:
					return _uint.ToString(format, formatProvider);
				case TypeCode.UInt64:
					return _ulong.ToString(format, formatProvider);
			}

			if (_typeCode == TypeCode.Object)
			{
				if (_object is IFormattable f)
				{
					return f.ToString(format, formatProvider);
				}
				else if (_object != null)
				{
					return _object.ToString();
				}
			}

			return $"{GetType().Name}({_typeCode})";
		}

		#endregion

		#region IConvertible

		/// <inheritdoc/>
		public TypeCode GetTypeCode()
		{
			return _typeCode;
		}

		/// <inheritdoc/>
		public bool ToBoolean(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Boolean)
			{
				return _bool;
			}

			switch (_typeCode)
			{
				case TypeCode.Byte:
					return Convert.ToBoolean(_byte);
				case TypeCode.Char:
					return Convert.ToBoolean(_char);
				case TypeCode.DateTime:
					return Convert.ToBoolean(_dateTime);
				case TypeCode.Double:
					return Convert.ToBoolean(_double);
				case TypeCode.Int16:
					return Convert.ToBoolean(_short);
				case TypeCode.Int32:
					return Convert.ToBoolean(_int);
				case TypeCode.Int64:
					return Convert.ToBoolean(_long);
				case TypeCode.Object:
					return Convert.ToBoolean(_object, provider);
				case TypeCode.SByte:
					return Convert.ToBoolean(_sbyte);
				case TypeCode.Single:
					return Convert.ToBoolean(_float);
				case TypeCode.String:
					return Convert.ToBoolean(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToBoolean(_ushort);
				case TypeCode.UInt32:
					return Convert.ToBoolean(_uint);
				case TypeCode.UInt64:
					return Convert.ToBoolean(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public byte ToByte(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Byte)
			{
				return _byte;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToByte(_bool);
				case TypeCode.Char:
					return Convert.ToByte(_char);
				case TypeCode.DateTime:
					return Convert.ToByte(_dateTime);
				case TypeCode.Double:
					return Convert.ToByte(_double);
				case TypeCode.Int16:
					return Convert.ToByte(_short);
				case TypeCode.Int32:
					return Convert.ToByte(_int);
				case TypeCode.Int64:
					return Convert.ToByte(_long);
				case TypeCode.Object:
					return Convert.ToByte(_object, provider);
				case TypeCode.SByte:
					return Convert.ToByte(_sbyte);
				case TypeCode.Single:
					return Convert.ToByte(_float);
				case TypeCode.String:
					return Convert.ToByte(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToByte(_ushort);
				case TypeCode.UInt32:
					return Convert.ToByte(_uint);
				case TypeCode.UInt64:
					return Convert.ToByte(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public char ToChar(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Char)
			{
				return _char;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToChar(_bool);
				case TypeCode.Byte:
					return Convert.ToChar(_byte);
				case TypeCode.DateTime:
					return Convert.ToChar(_dateTime);
				case TypeCode.Double:
					return Convert.ToChar(_double);
				case TypeCode.Int16:
					return Convert.ToChar(_short);
				case TypeCode.Int32:
					return Convert.ToChar(_int);
				case TypeCode.Int64:
					return Convert.ToChar(_long);
				case TypeCode.Object:
					return Convert.ToChar(_object, provider);
				case TypeCode.SByte:
					return Convert.ToChar(_sbyte);
				case TypeCode.Single:
					return Convert.ToChar(_float);
				case TypeCode.String:
					return Convert.ToChar(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToChar(_ushort);
				case TypeCode.UInt32:
					return Convert.ToChar(_uint);
				case TypeCode.UInt64:
					return Convert.ToChar(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public DateTime ToDateTime(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.DateTime)
			{
				return _dateTime;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToDateTime(_bool);
				case TypeCode.Byte:
					return Convert.ToDateTime(_byte);
				case TypeCode.Char:
					return Convert.ToDateTime(_char);
				case TypeCode.Double:
					return Convert.ToDateTime(_double);
				case TypeCode.Int16:
					return Convert.ToDateTime(_short);
				case TypeCode.Int32:
					return Convert.ToDateTime(_int);
				case TypeCode.Int64:
					return Convert.ToDateTime(_long);
				case TypeCode.Object:
					return Convert.ToDateTime(_object, provider);
				case TypeCode.SByte:
					return Convert.ToDateTime(_sbyte);
				case TypeCode.Single:
					return Convert.ToDateTime(_float);
				case TypeCode.String:
					return Convert.ToDateTime(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToDateTime(_ushort);
				case TypeCode.UInt32:
					return Convert.ToDateTime(_uint);
				case TypeCode.UInt64:
					return Convert.ToDateTime(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public decimal ToDecimal(IFormatProvider provider)
		{
			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToDecimal(_bool);
				case TypeCode.Byte:
					return Convert.ToDecimal(_byte);
				case TypeCode.Char:
					return Convert.ToDecimal(_char);
				case TypeCode.DateTime:
					return Convert.ToDecimal(_dateTime);
				case TypeCode.Double:
					return Convert.ToDecimal(_double);
				case TypeCode.Int16:
					return Convert.ToDecimal(_short);
				case TypeCode.Int32:
					return Convert.ToDecimal(_int);
				case TypeCode.Int64:
					return Convert.ToDecimal(_long);
				case TypeCode.Object:
					return Convert.ToDecimal(_object, provider);
				case TypeCode.SByte:
					return Convert.ToDecimal(_sbyte);
				case TypeCode.Single:
					return Convert.ToDecimal(_float);
				case TypeCode.String:
					return Convert.ToDecimal(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToDecimal(_ushort);
				case TypeCode.UInt32:
					return Convert.ToDecimal(_uint);
				case TypeCode.UInt64:
					return Convert.ToDecimal(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public double ToDouble(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Double)
			{
				return _double;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToDouble(_bool);
				case TypeCode.Byte:
					return Convert.ToDouble(_byte);
				case TypeCode.Char:
					return Convert.ToDouble(_char);
				case TypeCode.DateTime:
					return Convert.ToDouble(_dateTime);
				case TypeCode.Int16:
					return Convert.ToDouble(_short);
				case TypeCode.Int32:
					return Convert.ToDouble(_int);
				case TypeCode.Int64:
					return Convert.ToDouble(_long);
				case TypeCode.Object:
					return Convert.ToDouble(_object, provider);
				case TypeCode.SByte:
					return Convert.ToDouble(_sbyte);
				case TypeCode.Single:
					return Convert.ToDouble(_float);
				case TypeCode.String:
					return Convert.ToDouble(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToDouble(_ushort);
				case TypeCode.UInt32:
					return Convert.ToDouble(_uint);
				case TypeCode.UInt64:
					return Convert.ToDouble(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public short ToInt16(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Int16)
			{
				return _short;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToInt16(_bool);
				case TypeCode.Byte:
					return Convert.ToInt16(_byte);
				case TypeCode.Char:
					return Convert.ToInt16(_char);
				case TypeCode.DateTime:
					return Convert.ToInt16(_dateTime);
				case TypeCode.Double:
					return Convert.ToInt16(_double);
				case TypeCode.Int32:
					return Convert.ToInt16(_int);
				case TypeCode.Int64:
					return Convert.ToInt16(_long);
				case TypeCode.Object:
					return Convert.ToInt16(_object, provider);
				case TypeCode.SByte:
					return Convert.ToInt16(_sbyte);
				case TypeCode.Single:
					return Convert.ToInt16(_float);
				case TypeCode.String:
					return Convert.ToInt16(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToInt16(_ushort);
				case TypeCode.UInt32:
					return Convert.ToInt16(_uint);
				case TypeCode.UInt64:
					return Convert.ToInt16(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public int ToInt32(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Int32)
			{
				return _int;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToInt32(_bool);
				case TypeCode.Byte:
					return Convert.ToInt32(_byte);
				case TypeCode.Char:
					return Convert.ToInt32(_char);
				case TypeCode.DateTime:
					return Convert.ToInt32(_dateTime);
				case TypeCode.Double:
					return Convert.ToInt32(_double);
				case TypeCode.Int16:
					return Convert.ToInt32(_short);
				case TypeCode.Int64:
					return Convert.ToInt32(_long);
				case TypeCode.Object:
					return Convert.ToInt32(_object, provider);
				case TypeCode.SByte:
					return Convert.ToInt32(_sbyte);
				case TypeCode.Single:
					return Convert.ToInt32(_float);
				case TypeCode.String:
					return Convert.ToInt32(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToInt32(_ushort);
				case TypeCode.UInt32:
					return Convert.ToInt32(_uint);
				case TypeCode.UInt64:
					return Convert.ToInt32(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public long ToInt64(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Int64)
			{
				return _long;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToInt64(_bool);
				case TypeCode.Byte:
					return Convert.ToInt64(_byte);
				case TypeCode.Char:
					return Convert.ToInt64(_char);
				case TypeCode.DateTime:
					return Convert.ToInt64(_dateTime);
				case TypeCode.Double:
					return Convert.ToInt64(_double);
				case TypeCode.Int16:
					return Convert.ToInt64(_short);
				case TypeCode.Int32:
					return Convert.ToInt64(_int);
				case TypeCode.Object:
					return Convert.ToInt64(_object, provider);
				case TypeCode.SByte:
					return Convert.ToInt64(_sbyte);
				case TypeCode.Single:
					return Convert.ToInt64(_float);
				case TypeCode.String:
					return Convert.ToInt64(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToInt64(_ushort);
				case TypeCode.UInt32:
					return Convert.ToInt64(_uint);
				case TypeCode.UInt64:
					return Convert.ToInt64(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public sbyte ToSByte(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.SByte)
			{
				return _sbyte;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToSByte(_bool);
				case TypeCode.Byte:
					return Convert.ToSByte(_byte);
				case TypeCode.Char:
					return Convert.ToSByte(_char);
				case TypeCode.DateTime:
					return Convert.ToSByte(_dateTime);
				case TypeCode.Double:
					return Convert.ToSByte(_double);
				case TypeCode.Int16:
					return Convert.ToSByte(_short);
				case TypeCode.Int32:
					return Convert.ToSByte(_int);
				case TypeCode.Int64:
					return Convert.ToSByte(_long);
				case TypeCode.Object:
					return Convert.ToSByte(_object, provider);
				case TypeCode.Single:
					return Convert.ToSByte(_float);
				case TypeCode.String:
					return Convert.ToSByte(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToSByte(_ushort);
				case TypeCode.UInt32:
					return Convert.ToSByte(_uint);
				case TypeCode.UInt64:
					return Convert.ToSByte(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public float ToSingle(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.Single)
			{
				return _float;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToSingle(_bool);
				case TypeCode.Byte:
					return Convert.ToSingle(_byte);
				case TypeCode.Char:
					return Convert.ToSingle(_char);
				case TypeCode.DateTime:
					return Convert.ToSingle(_dateTime);
				case TypeCode.Double:
					return Convert.ToSingle(_double);
				case TypeCode.Int16:
					return Convert.ToSingle(_short);
				case TypeCode.Int32:
					return Convert.ToSingle(_int);
				case TypeCode.Int64:
					return Convert.ToSingle(_long);
				case TypeCode.Object:
					return Convert.ToSingle(_object, provider);
				case TypeCode.SByte:
					return Convert.ToSingle(_sbyte);
				case TypeCode.String:
					return Convert.ToSingle(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToSingle(_ushort);
				case TypeCode.UInt32:
					return Convert.ToSingle(_uint);
				case TypeCode.UInt64:
					return Convert.ToSingle(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public string ToString(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.String)
			{
				return _string;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToString(_bool, provider);
				case TypeCode.Byte:
					return Convert.ToString(_byte, provider);
				case TypeCode.Char:
					return Convert.ToString(_char, provider);
				case TypeCode.DateTime:
					return Convert.ToString(_dateTime, provider);
				case TypeCode.Double:
					return Convert.ToString(_double, provider);
				case TypeCode.Int16:
					return Convert.ToString(_short, provider);
				case TypeCode.Int32:
					return Convert.ToString(_int, provider);
				case TypeCode.Int64:
					return Convert.ToString(_long, provider);
				case TypeCode.Object:
					return Convert.ToString(_object, provider);
				case TypeCode.SByte:
					return Convert.ToString(_sbyte, provider);
				case TypeCode.Single:
					return Convert.ToString(_float, provider);
				case TypeCode.UInt16:
					return Convert.ToString(_ushort, provider);
				case TypeCode.UInt32:
					return Convert.ToString(_uint, provider);
				case TypeCode.UInt64:
					return Convert.ToString(_ulong, provider);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public ushort ToUInt16(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.UInt16)
			{
				return _ushort;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToUInt16(_bool);
				case TypeCode.Byte:
					return Convert.ToUInt16(_byte);
				case TypeCode.Char:
					return Convert.ToUInt16(_char);
				case TypeCode.DateTime:
					return Convert.ToUInt16(_dateTime);
				case TypeCode.Double:
					return Convert.ToUInt16(_double);
				case TypeCode.Int16:
					return Convert.ToUInt16(_short);
				case TypeCode.Int32:
					return Convert.ToUInt16(_int);
				case TypeCode.Int64:
					return Convert.ToUInt16(_long);
				case TypeCode.Object:
					return Convert.ToUInt16(_object, provider);
				case TypeCode.SByte:
					return Convert.ToUInt16(_sbyte);
				case TypeCode.Single:
					return Convert.ToUInt16(_float);
				case TypeCode.String:
					return Convert.ToUInt16(_object, provider);
				case TypeCode.UInt32:
					return Convert.ToUInt16(_uint);
				case TypeCode.UInt64:
					return Convert.ToUInt16(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public uint ToUInt32(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.UInt32)
			{
				return _uint;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToUInt32(_bool);
				case TypeCode.Byte:
					return Convert.ToUInt32(_byte);
				case TypeCode.Char:
					return Convert.ToUInt32(_char);
				case TypeCode.DateTime:
					return Convert.ToUInt32(_dateTime);
				case TypeCode.Double:
					return Convert.ToUInt32(_double);
				case TypeCode.Int16:
					return Convert.ToUInt32(_short);
				case TypeCode.Int32:
					return Convert.ToUInt32(_int);
				case TypeCode.Int64:
					return Convert.ToUInt32(_long);
				case TypeCode.Object:
					return Convert.ToUInt32(_object, provider);
				case TypeCode.SByte:
					return Convert.ToUInt32(_sbyte);
				case TypeCode.Single:
					return Convert.ToUInt32(_float);
				case TypeCode.String:
					return Convert.ToUInt32(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToUInt32(_ushort);
				case TypeCode.UInt64:
					return Convert.ToUInt32(_ulong);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public ulong ToUInt64(IFormatProvider provider)
		{
			if (_typeCode == TypeCode.UInt64)
			{
				return _ulong;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return Convert.ToUInt64(_bool);
				case TypeCode.Byte:
					return Convert.ToUInt64(_byte);
				case TypeCode.Char:
					return Convert.ToUInt64(_char);
				case TypeCode.DateTime:
					return Convert.ToUInt64(_dateTime);
				case TypeCode.Double:
					return Convert.ToUInt64(_double);
				case TypeCode.Int16:
					return Convert.ToUInt64(_short);
				case TypeCode.Int32:
					return Convert.ToUInt64(_int);
				case TypeCode.Int64:
					return Convert.ToUInt64(_long);
				case TypeCode.Object:
					return Convert.ToUInt64(_object, provider);
				case TypeCode.SByte:
					return Convert.ToUInt64(_sbyte);
				case TypeCode.Single:
					return Convert.ToUInt64(_float);
				case TypeCode.String:
					return Convert.ToUInt64(_object, provider);
				case TypeCode.UInt16:
					return Convert.ToUInt64(_ushort);
				case TypeCode.UInt32:
					return Convert.ToUInt64(_uint);
			}

			throw new InvalidCastException();
		}

		/// <inheritdoc/>
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(bool))
			{
				return ToBoolean(provider);
			}

			if (conversionType == typeof(char))
			{
				return ToChar(provider);
			}

			if (conversionType == typeof(byte))
			{
				return ToByte(provider);
			}

			if (conversionType == typeof(sbyte))
			{
				return ToSByte(provider);
			}

			if (conversionType == typeof(short))
			{
				return ToInt16(provider);
			}

			if (conversionType == typeof(ushort))
			{
				return ToUInt16(provider);
			}

			if (conversionType == typeof(int))
			{
				return ToInt32(provider);
			}

			if (conversionType == typeof(uint))
			{
				return ToUInt32(provider);
			}

			if (conversionType == typeof(long))
			{
				return ToInt64(provider);
			}

			if (conversionType == typeof(ulong))
			{
				return ToUInt64(provider);
			}

			if (conversionType == typeof(float))
			{
				return ToSingle(provider);
			}

			if (conversionType == typeof(double))
			{
				return ToDouble(provider);
			}

			if (conversionType == typeof(decimal))
			{
				return ToDecimal(provider);
			}

			if (conversionType == typeof(string))
			{
				return ToString(provider);
			}

			if (conversionType == typeof(DateTime))
			{
				return ToDateTime(provider);
			}

			if (_typeCode == TypeCode.Object)
			{
				if (_object != null)
				{
					if (conversionType.IsAssignableFrom(_object.GetType()))
					{
						return _object;
					}
					else if (_object is IConvertible c)
					{
						return c.ToType(conversionType, provider);
					}
				}
			}

			throw new InvalidCastException();
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			if (_typeCode == TypeCode.String)
			{
				return _string;
			}

			switch (_typeCode)
			{
				case TypeCode.Boolean:
					return _bool.ToString();
				case TypeCode.Byte:
					return _byte.ToString();
				case TypeCode.Char:
					return _char.ToString();
				case TypeCode.DateTime:
					return _dateTime.ToString();
				case TypeCode.Double:
					return _double.ToString();
				case TypeCode.Int16:
					return _short.ToString();
				case TypeCode.Int32:
					return _int.ToString();
				case TypeCode.Int64:
					return _long.ToString();
				case TypeCode.SByte:
					return _sbyte.ToString();
				case TypeCode.Single:
					return _float.ToString();
				case TypeCode.UInt16:
					return _ushort.ToString();
				case TypeCode.UInt32:
					return _uint.ToString();
				case TypeCode.UInt64:
					return _ulong.ToString();
				case TypeCode.Object:
					return _object.ToString();
			}

			return $"{GetType().Name}({_typeCode})";
		}

		#endregion
	}
}
