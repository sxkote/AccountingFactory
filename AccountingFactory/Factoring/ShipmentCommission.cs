using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Factoring
{
    /// <summary>
    /// Комиссия в накладной
    /// </summary>
    public struct ShipmentCommission
    {
        public const double NDS = 0.18;

        #region variables
        private Money _body;
        private Money _commission;
        private Money _standart;
        private Money _extra;
        private Money _daily;
        private Money _peny;
        private Money _mit;
        private Money _increased;
        #endregion

        /// <summary>
        /// MoneyFinancing = Погашение задолженности (сумма погашаемого тела кредита, относительно которого считается комиссия)
        /// </summary>
        public Money Body
        { get { return _body; } }

        /// <summary>
        /// MoneyCommission = КП2 (комиссия % от суммы задолженности)
        /// </summary> 
        public Money Commission
        { get { return _commission; } }

        /// <summary>
        /// MoneyResoursesStandart = КП1 (комиссия %%годовых на сумму кредита)
        /// </summary>
        public Money Standart
        { get { return _standart; } }

        /// <summary>
        /// MoneyResoursesExtra = КП1 Экстра (комиссия %%годовых за просрочку)
        /// </summary>
        public Money Extra
        { get { return _extra; } }

        /// <summary>
        /// MoneyPenalty = КП3 (комиссия %% от суммы задолженности за каждый день)
        /// </summary>
        public Money Daily
        { get { return _daily; } }

        /// <summary>
        /// MoneyPeny = Пени (просто, пени...)
        /// </summary>
        public Money Peny
        { get { return _peny; } }

        /// <summary>
        /// MoneyAddonCommission = Комиссия МИТ (комиссия МеталлИнвестТехнологии)
        /// </summary>
        public Money MIT
        { get { return _mit; } }

        /// <summary>
        /// MoneyAddonPenalty = Наращенная Комиссия (сколько используется)
        /// </summary>
        public Money Increased
        { 
            get { return _increased; } 
            set{_increased = value.NotMore(this.TotalClear);}
        }

        /// <summary>
        /// Сколько нужно донарастить в рамках данной операции
        /// </summary>
        public Money IncreseNeeded
        { get { return this.TotalClear - this.Increased; } }

        /// <summary>
        /// MoneyShadowCommission = Сумма Всей Комиссии
        /// </summary>
        public Money Total
        { get { return _commission + _standart + _extra + _daily + _peny; } }

        public Money TotalNDS
        { get { return this.Total.Value * NDS / (1 + NDS); } }

        public Money TotalClear
        { get { return (this.Total - this.TotalNDS).NotLess(0); } }

        public ShipmentCommission(Money body, Money commission, Money standart, Money extra, Money daily, Money peny, Money mit)
        {
            _body = body;
            _commission = commission;
            _standart = standart;
            _extra = extra;
            _daily = daily;
            _peny = peny;
            _mit = mit;
            _increased = 0;
        }

        public ShipmentCommission(Money body, Money commission, Money standart, Money extra, Money daily, Money peny)
            : this(body, commission, standart, extra, daily, peny, 0) { }

        public ShipmentCommission(Money body, Money commission, Money standart, Money extra, Money daily)
            : this(body, commission, standart, extra, daily, 0, 0) { }
    }
}
