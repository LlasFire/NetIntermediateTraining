using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public static class EnumerableExtension
    {
        public static IEnumerable<byte[]> Split(this byte[] value, int bufferLength)
        {
            var countOfArray = value.NumberOfParts(bufferLength);

            for (var i = 0; i < countOfArray; i++)
            {
                yield return value.Skip(i * bufferLength).Take(bufferLength).ToArray();

            }
        }

        public static int NumberOfParts(this byte[] value, int bufferLength)
        {
            var countOfArray = value.Length / bufferLength;

            if (value.Length % bufferLength > 0)
            {
                countOfArray++;
            }

            return countOfArray;
        }
    }
}
