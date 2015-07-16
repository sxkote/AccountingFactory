using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    /// <summary>
    /// Предстваление суммы денег
    /// </summary>
    public struct Money
    {
        public const int Precision = 2;

        private double _value;

        public double Value
        { get { return _value; } }

        public Money(double value)
        {
            //_value = Math.Round(Math.Max(0, value), Precision);
            _value = Math.Round(value, Precision);
        }

        public Money NotLess(Money min)
        { return this < min ? min : this; }

        public Money NotMore(Money max)
        { return this > max ? max : this; }

        #region Functions
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Money)
                return this.Value.Equals(((Money)obj).Value);

            return this.Value.Equals(obj);
        }
        #endregion

        #region Operators
        static public implicit operator Money(decimal value)
        { return new Money((double)value); }

        static public implicit operator Money(double value)
        { return new Money(value); }

        static public implicit operator Money(int value)
        { return new Money(value); }

        static public implicit operator double(Money value)
        { return value.Value; }

        public static Money operator +(Money first, Money second)
        { return new Money(first.Value + second.Value); }

        public static Money operator -(Money first, Money second)
        { return new Money(first.Value - second.Value); }

        public static Money operator /(Money first, double second)
        { return first.Value / second; }

        public static double operator /(Money first, Money second)
        { return first.Value / second.Value; }

        public static bool operator ==(Money first, Money second)
        {
            return first.Value == second.Value;
        }

        public static bool operator !=(Money first, Money second)
        {
            return first.Value != second.Value;
        }

        public static bool operator >(Money first, Money second)
        {
            return first.Value > second.Value;
        }

        public static bool operator <(Money first, Money second)
        {
            return first.Value < second.Value;
        }

        public static bool operator >=(Money first, Money second)
        {
            return first.Value >= second.Value;
        }

        public static bool operator <=(Money first, Money second)
        {
            return first.Value <= second.Value;
        }
        #endregion
    }
}
