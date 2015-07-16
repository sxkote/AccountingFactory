using AccountingFactory;
using AccountingFactory.Factoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactoryTests
{
    /// <summary>
    /// Пример того, как может выглядеть объект Shipment
    /// Все параметры этот объект может брать из базы
    /// </summary>
    public class ExampleShipment
    {
        public List<ShipmentOperation> Operations { get; set; }

        public string ContractFactoring { get; set; }
        public string ContractDelivery { get; set; }

        public string Title { get; set; }
        public double Summ { get; set; }

        public DateTime DateShipment { get; set; }
        public DateTime DatePayment { get; set; }
        
        public bool StatusRegress { get; set; }
        public bool StatusFinancing { get; set; }

        public double RateFinancing { get; set; }

        public double MITComFin { get; set; }
        public double MITComNoFin { get; set; }
        public double MITComMin { get; set; }

        public bool CommissionCommonPassing { get; set; }

        public double PrepayCommission { get; set; }
        public double RateCommission { get; set; }
        public double RateCommissionMin { get; set; }
        
        public double RateStandart { get; set; }
        public double RateExtra { get; set; }
        
        public double RateValue { get; set; }
        public double RateValueMin { get; set; }
        public double RateValuePlus { get; set; }

        public double RatePeny { get; set; }

        public bool IsAccountCommission407 { get; set; }

        public double DutyCustomer
        {
            get
            {
                return this.Operations.Where(o => o.Type == "CustomerFinansing" || o.Type == "CustomerFinancing").Sum(o => o.Summ);
            }
        }

        public ExampleShipment()
        {
            Operations = new List<ShipmentOperation>();
        }
    }
}
