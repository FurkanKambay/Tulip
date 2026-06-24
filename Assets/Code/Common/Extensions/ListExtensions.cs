using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Furkan.Common.Extensions
{
    [PublicAPI]
    public static class ListExtensions
    {
        private static RNGCryptoServiceProvider rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                byte[] box = new byte[1];

                do
                    rng.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));

                int k = box[0] % n;
                n--;

                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
