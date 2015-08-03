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
        /// <summary>
        /// Номинал накладной
        /// </summary>
        public double Summ { get; set; }

        /// <summary>
        /// Дата поставки
        /// </summary>
        public DateTime DateShipment { get; set; }
        /// <summary>
        /// Дата платежа
        /// </summary>
        public DateTime DatePayment { get; set; }
        
        /// <summary>
        /// Договор с регрессом / без
        /// </summary>
        public bool StatusRegress { get; set; }
        /// <summary>
        /// С финансированием / без
        /// </summary>
        public bool StatusFinancing { get; set; }

        /// <summary>
        /// % дострочного платежа
        /// </summary>
        public double RateFinancing { get; set; }

        /// <summary>
        /// Комиссия МИТ при выплате с финансированием
        /// </summary>
        public double MITComFin { get; set; }
        /// <summary>
        /// Комиссия МИТ при обработке платежа
        /// </summary>
        public double MITComNoFin { get; set; }
        /// <summary>
        /// Минимальная комиссия МИТ
        /// </summary>
        public double MITComMin { get; set; }

        public bool CommissionCommonPassing { get; set; }

        public double PrepayCommission { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
