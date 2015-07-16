using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    /// <summary>
    /// Ставка конвертации валюты
    /// </summary>
    public struct RateCurrency
    {
        private DateTime _date;
        private double _rate;
        private double _amendment;

        /// <summary>
        /// Дата, на которую рассчитывалась (бралась) ставка
        /// </summary>
        public DateTime Date
        { get { return _date; } }

        /// <summary>
        /// Ставка (стоимость) валюты по онтношению к основной валюте 
        /// <example>Например, для USD (доллара) на 15.07.2015 Rate = 56.97</example>
        /// <example>Например, для EUR (евро) на 15.07.2015 Rate = 62.62</example>
        /// </summary>
        public double Rate
        { get { return _rate; } }

        /// <summary>
        /// Коэффициент поправки к основному курсу валюты
        /// </summary>
        public double Amendment
        { get { return _amendment; } }

        public RateCurrency(DateTime date, double rate, double amendment = 0)
        {
            _date = date;
            _rate = rate;
            _amendment = amendment;
        }

        /// <summary>
        /// Конвертировать основную валюту (рубли) в международную (например в USD)
        /// </summary>
        /// <param name="money">Сумма конвертации в основной валюте (в рублях)</param>
        /// <returns>Эквивалент суммы в международной валюте (например в USD)</returns>
        public Money ToCurrency(Money money)
        {
            return money / this.Rate * (1 + this.Amendment);
        }

        /// <summary>
        /// Конвертировать международную валюту (например в USD) в основную (рубли) 
        /// </summary>
        /// <param name="currency">Сумма международной валюты (например USD) для конвертации</param>
        /// <returns>Эквивалент международной валюты в основной, то есть в рублях</returns>
        public Money FromCurrency(Money currency)
        {
            return currency * this.Rate;
        }
    }
}
