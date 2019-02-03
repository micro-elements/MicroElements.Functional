using System;
using System.Linq.Expressions;

namespace MicroElements.Functional.Tests
{
    public static class Static
    {
        /// <summary>
        /// GetValue helper.
        /// </summary>
        public static Func<T> GetValue<T>(Expression<Func<T>> getValue) => getValue.Compile();
    }
}
