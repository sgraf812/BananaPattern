using System;
using BananaPattern.Operators;
using BananaPattern;

namespace BananaTest
{
    internal class StubOperator : Operator
    {
        public bool IsExecuted { get; private set; }

        public StubOperator()
            : base(IntPtr.Zero)
        {
        }

        public StubOperator(IntPtr target)
            : base(target)
        {
        }

        public StubOperator(Func<string> valueFactory, Func<string> targetFactory)
            : base(targetFactory)
        {
            IsExecuted = true;
        }

        public override IntPtr Execute(IMemory memory)
        {
            IsExecuted = true;
            return Target;
        }

        public override bool IsCacheable
        {
            get { return true; }
        }
    }
}
