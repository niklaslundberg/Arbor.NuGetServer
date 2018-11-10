using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.CommonExtensions
{
    public static class EnumerableExtensions
    {
        public static ImmutableArray<T> ThrowIfDefault<T>(this ImmutableArray<T> array)
        {
            if (array.IsDefault)
            {
                throw new InvalidOperationException($"The immutable array of {typeof(T).FullName} must not be default");
            }

            return array;
        }

        public static ImmutableArray<T> SafeToImmutableArray<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                return ImmutableArray<T>.Empty;
            }

            if (items is ImmutableArray<T> array)
            {
                return array.IsDefault ? ImmutableArray<T>.Empty : array;
            }

            return items.ToImmutableArray();
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

            foreach (T item in items)
            {
                action(item);
            }
        }
    }
}
