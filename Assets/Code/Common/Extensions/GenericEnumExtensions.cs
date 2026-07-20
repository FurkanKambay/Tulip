using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace FK.Common.Extensions
{
    /// Source: https://github.com/dotnet/csharplang/discussions/1993#discussioncomment-104840s
    [PublicAPI]
    public static class GenericEnumExtensions
    {
        public static TNum As<TNum, TEnum>(this TEnum enumValue)
            where TEnum : Enum
            where TNum : struct, IComparable, IComparable<TNum>, IEquatable<TNum>
        {
            if (Unsafe.SizeOf<TEnum>() != Unsafe.SizeOf<TNum>())
                throw new ArgumentException($"Size mismatch when casting {typeof(TEnum).Name} to {typeof(TNum).Name}.");

            TNum value = Unsafe.As<TEnum, TNum>(ref enumValue);
            return value;
        }

        public static long AsLong<TEnum>(this TEnum enumValue)
            where TEnum : Enum
        {
            long value;

            int enumSize = Unsafe.SizeOf<TEnum>();

            if (enumSize == Unsafe.SizeOf<byte>())
                value = Unsafe.As<TEnum, byte>(ref enumValue);
            else if (enumSize == Unsafe.SizeOf<short>())
                value = Unsafe.As<TEnum, short>(ref enumValue);
            else if (enumSize == Unsafe.SizeOf<int>())
                value = Unsafe.As<TEnum, int>(ref enumValue);
            else if (enumSize == Unsafe.SizeOf<long>())
                value = Unsafe.As<TEnum, long>(ref enumValue);

            else throw new ArgumentException($"Size mismatch when casting {typeof(TEnum).Name} to long.");

            return value;
        }
    }
}
