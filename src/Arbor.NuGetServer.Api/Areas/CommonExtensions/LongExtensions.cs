using System;

namespace Arbor.NuGetServer.Api.Areas.CommonExtensions
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
                string rounded = (value / (double)Kibibyte).ToString("F");

                return $"{rounded} KiB";
            }

            if (value < Gigibyte)
            {
                string rounded = (value / (double)Mebibyte).ToString("F");

                return $"{rounded} MiB";
            }

            string gibibytes = (value / (double)Gigibyte).ToString("F");

            return $"{gibibytes} GiB";
        }
    }
}
