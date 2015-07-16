using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Values
{
    /// <summary>
    /// Represents the Text Value item from the Report 
    /// </summary>
    public class ValueText : Value
    {
        protected string _value;

        public string Value 
        { get { return _value; } }

        public override ValueType Type 
        { get { return ValueType.Text; } }

        public ValueText(string value)
        { _value = value; }

        public override string ToString()
        { return this.Value; }

        static public implicit operator string(ValueText text)
        { return text.Value; }

        static public implicit operator ValueText(string text)
        { return new ValueText(text); }
    }
}
