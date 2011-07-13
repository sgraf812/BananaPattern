using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Operators
{
    public enum LeaType 
    { 
        Byte, 
        Word, 
        Dword,
        Pointer
    }

    [Operator("Lea")]
    public class LeaOperator : Operator 
    {
        private Lazy<LeaType> _type;
        public LeaType Type
        {
            get { return _type.Value; }
            private set { _type = new Lazy<LeaType>(() => value); }
        }

        /// <summary>
        /// Initializes Target to 0 and Type to LeaType.Dword.
        /// </summary>
        public LeaOperator()
            : this(IntPtr.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="target">Initial Target value.</param>
        /// <param name="type">Initial Type value.</param>
        public LeaOperator(IntPtr target, LeaType type = LeaType.Dword)
            : base(target)
        {
            Type = type;
        }

        /// <summary>
        /// Attempts to lazy initialize a new instance from factories.
        /// </summary>
        /// <param name="valueFactory">Factory providing a parseable value for Type.</param>        
        /// <param name="targetFactory">Factory providing a parseable value for Target.</param>
        public LeaOperator(Func<string> valueFactory, Func<string> targetFactory)
            : base(targetFactory)
        {
            _type = new Lazy<LeaType>(() => ParseLeaType(valueFactory()));
        }

        private static LeaType ParseLeaType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return LeaType.Dword;
            }

            LeaType type;
            if (!Enum.TryParse(value, out type))
            {
                throw new PatternException("Unknown LeaType");
            }
            return type;
        }

        public override IntPtr Execute(IMemory memory)
        {
            switch (Type)
            {
                case LeaType.Byte:
                    return (IntPtr)memory.Read<byte>(Target);

                case LeaType.Word:
                    return (IntPtr)memory.Read<ushort>(Target);

                case LeaType.Dword:
                    return (IntPtr)memory.Read<uint>(Target);

                case LeaType.Pointer:
                    return memory.Read<IntPtr>(Target);

                default:
                    throw new PatternException("Unknown LeaType"); 
            }
        }

        public override bool IsCacheable { get { return false; } }
    }
}
