using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Values
{
    /// <summary>
    /// Represents the Boolean Value item
    /// </summary>
    public class ValueBool : Value
    {
        protected bool _value;

        public bool Value
        { get { return _value; } }

        public override ValueType Type
        { get { return ValueType.Bool; } }

        public ValueBool(bool value)
        { _value = value; }

        public override string ToString()
        { return _value ? "true" : "false"; }

        static public implicit operator ValueBool(bool flag)
        { return new ValueBool(flag); }

        static public implicit operator bool(ValueBool value)
        { return value.Value; }
    }
}
