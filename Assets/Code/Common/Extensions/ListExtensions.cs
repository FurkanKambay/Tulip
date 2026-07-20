using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

namespace FK.Common.Extensions
{
    [PublicAPI]
    public static class ListExtensions
    {
        public static void ShuffleFast<T>(this IList<T> list)
        {
            for (int index = list.Count - 1; index > 0; index--)
            {
                int newIndex = Random.Range(0, index + 1);
                (list[newIndex], list[index]) = (list[index], list[newIndex]);
            }
        }

        public static void ShuffleSecure<T>(this IList<T> list)
        {
            int count = list.Count;
            if (count > 255)
                throw new ArgumentOutOfRangeException(nameof(list), $"Max 255 elements supported (got {count}).");

            Span<byte> box = stackalloc byte[1];

            for (int index = count - 1; index > 0; --index)
            {
                int range = index + 1; // 0..index
                byte limit = (byte)(range * (byte.MaxValue / range)); // for unbiased modulo

                do RandomNumberGenerator.Fill(box);
                while (box[0] >= limit);

                int newIndex = box[0] % range;
                (list[newIndex], list[index]) = (list[index], list[newIndex]);
            }
        }
    }
}
