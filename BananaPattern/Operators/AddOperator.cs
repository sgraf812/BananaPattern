using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Operators
{
    [Operator("Add")]
    public class AddOperator : Operator
    {
        private Lazy<int> _offset;
        public int Offset
        {
            get { return _offset.Value; }
            private set { _offset = new Lazy<int>(() => value); }
        }
        
        /// <summary>
        /// Initializes a new instance. The target and the offset will be 0.
        /// </summary>
        public AddOperator()
            : this(IntPtr.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="target">Initial Target value.</param>
        /// <param name="offset">Initial Offset value.</param>
        public AddOperator(IntPtr target, int offset = 0)
            : base(target)
        {
            Offset = offset;
        }

        /// <summary>
        /// Attempts to lazy initialize a new instance from factories.
        /// </summary>
        /// <param name="valueFactory">Factory providing a parseable value for Offset.</param>
        /// <param name="targetFactory">Factory providing a parseable value for Target.</param>
        public AddOperator(Func<string> valueFactory, Func<string> targetFactory)
            : base(targetFactory)
        {
            _offset = new Lazy<int>(() => ParseOffset(valueFactory()));
        }

        private static int ParseOffset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(value, 0x10);
            }
        }

        public override IntPtr Execute(IMemory memory)
        {
            return Target + Offset;
        }

        public override bool IsCacheable { get { return true; } }
    }
}
