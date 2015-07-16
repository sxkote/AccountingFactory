using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Factoring
{
    public struct ShipmentOperationPartToCustomer
    {
        private DateTime _date;
        private Money _summ;
        private ShipmentOperationPartNetting _netting;

        public DateTime Date
        { get { return _date; } }

        public Money Summ
        { get { return _summ; } }

        public ShipmentOperationPartNetting Netting
        { get { return _netting; } }

        public Money Send
        { get { return (this.Summ - this.Netting.Summ).NotMore(this.Summ).NotLess(0); } }

        public ShipmentOperationPartToCustomer(DateTime date, Money summ, ShipmentOperationPartNetting netting)
        {
            _date = date;
            _summ = summ;
            _netting = netting;
        }

        public ShipmentOperationPartToCustomer(DateTime date, Money summ)
        {
            _date = date;
            _summ = summ;
            _netting = new ShipmentOperationPartNetting();
        }
    }

    public struct ShipmentOperationPartNetting
    {
        private Money _summ;
        private Account _account;

        public Money Summ
        { get { return _summ; } }

        public Account Account
        { get { return _account; } }

        public ShipmentOperationPartNetting(Money summ, Account account)
        {
            _summ = summ;
            _account = account;
        }
    }
}
