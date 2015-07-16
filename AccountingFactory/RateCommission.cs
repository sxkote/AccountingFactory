using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    /// <summary>
    /// Комиссионная ставка
    /// </summary>
    public struct RateCommission
    {
        private double _rate;
        private double _fix;
        private double _min;
        private double _add;

        public double Rate
        { get { return _rate; } }

        public double Fix
        { get { return _fix; } }

        public double Min
        { get { return _min; } }

        public double Add
        { get { return _add; } }

        public RateCommission(double percent, double fix = 0, double min = 0, double add = 0)
        {
            _rate = percent;
            _fix = fix;
            _min = min;
            _add = add;
        }

        public Money Calculate(double summ)
        {
            return Math.Max(Min, Fix + summ * Rate) + Add;
        }

        static public RateCommission Zero
        { get { return new RateCommission(0, 0, 0, 0); } }
    }
}
