using System;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class LongExtensions
    {
        private const int Multiple = 1024;

        private const int Kibibyte = Multiple;

        private const int Gigibyte = Mebibyte * Kibibyte;

        private const int Mebibyte = Kibibyte * Kibibyte;

        public static string AsSuitableByteUnit(this long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value == 0)
            {
                return "0 byte";
            }

            if (value < Kibibyte)
            {
                return $"{value} bytes";
            }

            if (value < Mebibyte)
            {
                var rounded = (value / (double)Kibibyte).ToString("F");

                return $"{rounded} KiB";
            }

            if (value < Gigibyte)
            {
                var rounded = (value / (double)Mebibyte).ToString("F");

                return $"{rounded} MiB";
            }

            var gibibytes = (value / (double)Gigibyte).ToString("F");

            return $"{gibibytes} GiB";
        }
    }
}