using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountingFactory.Factoring
{
    public class ShipmentOperation : Operation
    {
        private ShipmentOperationPartToCustomer _toCustomer;
        private ShipmentCommission _commission;

        public ShipmentOperationPartToCustomer ToCustomer
        {
            get { return _toCustomer; }
            set { _toCustomer = value; }
        }

        public ShipmentCommission Commission
        {
            get { return _commission; }
            set { _commission = value; }
        }

        public ShipmentOperation(string type, DateTime date, double summ, string comment = "")
            : base(type, date, summ, comment) { }
    }
}
