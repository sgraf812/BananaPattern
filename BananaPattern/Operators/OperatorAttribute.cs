using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Operators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class OperatorAttribute : Attribute
    {
        public string Identifier { get; private set; }

        public OperatorAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}
