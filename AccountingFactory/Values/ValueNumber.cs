using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Values
{
    /// <summary>
    /// Represents the Numeric Value item from the Report 
    /// </summary>
    public class ValueNumber : Value
    {
        protected double _value;

        public double Value 
        { get { return _value; } }

        public override ValueType Type 
        { get { return ValueType.Number; } }

        public ValueNumber(double value)
        { _value = value; }

        public override string ToString()
        { return this.Value.ToString(); }

        static public implicit operator double(ValueNumber number)
        { return number.Value; }

        static public implicit operator ValueNumber(decimal number)
        { return new ValueNumber((double)number); }

        static public implicit operator ValueNumber(double number)
        { return new ValueNumber(number); }

        static public implicit operator ValueNumber(int number)
        { return new ValueNumber(number); }
    }
}
