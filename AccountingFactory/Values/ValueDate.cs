using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Values
{
    /// <summary>
    /// Represents the DateTime Value item from the Report 
    /// </summary>
    public class ValueDate : Value
    {
        public const string Format = "dd.MM.yyyy";

        protected DateTime _value;

        public DateTime Value 
        { get { return _value; } }

        public override ValueType Type 
        { get { return ValueType.Date; } }

        public ValueDate(DateTime value)
        { _value = value; }

        public override string ToString()
        { return this.Value.ToString(Format); }

        static public implicit operator ValueDate(DateTime date)
        { return new ValueDate(date); }

        static public implicit operator DateTime(ValueDate date)
        { return date.Value; }
    }
}
