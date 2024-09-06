using System.Collections.Generic;
using System.Linq;

namespace Homework.Infrastructure.Extensions
{
    public static class ListExtension
    {
        public static bool HasElement<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }
    }
}

