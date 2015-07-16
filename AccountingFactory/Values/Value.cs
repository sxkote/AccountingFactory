using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Values
{
    /// <summary>
    /// Represents the Value item from the Report (text value, number value, datetime value, ...)
    /// </summary>
    public abstract class Value
    {
        public enum ValueType { Text, Number, Date, Bool };

        public abstract ValueType Type { get; }

        static public implicit operator string (Value value)
        {
            if (value == null)
                return "";

            var text = value as ValueText;
            if (text != null)
                return text.Value;

            return value.ToString();
        }

        static public implicit operator Value(string value)
        { 
            return new ValueText(value); 
        }

        static public implicit operator double (Value value)
        {
            var number = Value.Convert(value, ValueType.Number) as ValueNumber;
            if (number != null)
                return number.Value;

            throw new ArgumentException("Can't convert Value to Double");
        }

        static public implicit operator Value(decimal value)
        { return new ValueNumber((double)value); }

        static public implicit operator Value(double value)
        { return new ValueNumber(value); }

        static public implicit operator Value(int value)
        { return new ValueNumber(value); }

        static public implicit operator Value(DateTime value)
        { return new ValueDate(value); }

        static public implicit operator DateTime(Value value)
        {
            var number = Value.Convert(value, ValueType.Date) as ValueDate;
            if (number != null)
                return number.Value;

            throw new ArgumentException("Can't convert Value to DateTime");
        }


        static public implicit operator Value(bool value)
        { return new ValueBool(value); }

        static public implicit operator bool(Value value)
        {
            var flag = Value.Convert(value, ValueType.Bool) as ValueBool;
            if (flag != null)
                return flag.Value;

            throw new ArgumentException("Can't convert Value to Boolean");
        }

        static public ValueType ParseValueType(string input)
        {
            switch (input.Trim().ToLower())
            {
                case "date":
                case "datetime":
                    return Value.ValueType.Date;

                case "bool":
                case "boolean":
                    return Value.ValueType.Date;

                case "number":
                case "int":
                case "double":
                    return Value.ValueType.Number;

                default: return Value.ValueType.Text;
            }
        }

        static public Value Convert(Value value, Value.ValueType type)
        {
            if (value == null)
                return null;

            if (value.Type == type)
                return value;

            switch (type)
            {
                case Value.ValueType.Number:
                    return Double.Parse(value.ToString());
                case Value.ValueType.Date:
                    return DateTime.ParseExact(value.ToString(), "dd.MM.yyyy", null);
                default:
                    return value.ToString();
            }
        }
    }
}