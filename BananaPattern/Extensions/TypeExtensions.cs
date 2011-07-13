using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Extensions
{
    static class TypeExtensions
    {
        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).Length != 0;
        }
    }
}
