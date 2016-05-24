using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IReadOnlyCollection<T> SafeToReadOnlyCollection<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                return new List<T>();
            }

            var list = items as List<T>;

            return list ?? items.ToList();
        }

        public static void ForEach<T>([NotNull] this IEnumerable<T> items, [NotNull] Action<T> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
        }
    }
}