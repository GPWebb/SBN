using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SBN.Lib.Sys
{
    public static class ListExtensions
    {
        public static bool IsIn<T>(this T item, IEnumerable<T> list) => list.Contains(item);
        public static bool IsIn<T>(this T item, params T[] list) => list.Contains(item);

        public static bool AnyExist<T>(this IEnumerable<T> items) => items?.Any() ?? false;

        //https://stackoverflow.com/questions/59380470/convert-iasyncenumerable-to-list - Nuget reference wasn't coming through for some reason?!
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            var results = new List<T>();
            await foreach (var item in items.WithCancellation(cancellationToken)
                                            .ConfigureAwait(false))
                results.Add(item);
            return results;
        }
    }
}
