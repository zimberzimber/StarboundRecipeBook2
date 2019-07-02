using System;
using System.Linq;

namespace StarboundRecipeBook2.Helpers
{
    public static class QueriableIf
    {
        // Taken from https://stackoverflow.com/questions/53474431/ef-core-linq-and-conditional-include-and-theninclude-problem
        public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> transform)
            => condition ? transform(source) : source;
    }
}
