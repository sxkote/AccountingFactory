using AccountingFactory;
using AccountingFactory.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactoryTests
{
    /// <summary>
    /// Пример того, как может выглядеть провайдер данных
    /// Он использует кэш, чтобы дважды не грузить данные
    /// Ставки вытаскиваются также через собственный механизм определения параметров!
    /// </summary>
    public class ExampleShipmentParamsProvider : IAccountingParamsProvider
    {
        private ExampleShipment _shipment;

        DateTime _date;
        double _summ;
        string _type;
      
        Dictionary<string, Value> _cacheValues;
        Dictionary<string, RateCommission> _cacheRates;
        Dictionary<string, Account> _cacheAccounts;

        private DateTime DateFinancing
        { get { return _shipment.Operations.Last(o => o.Type == "CustomerFinansing").Date; } }

        public Value this[string name]
        { get { return this.GetParam(name); } }

        public ExampleShipmentParamsProvider(ExampleShipment shipment, DateTime date, double summ, string type = "")
        {
            _shipment = shipment;

            _date = date;
            _summ = summ;
            _type = type;
        
            _cacheValues = new Dictionary<string, Value>();
            _cacheRates = new Dictionary<string, RateCommission>();
            _cacheAccounts = new Dictionary<string, Account>();
        }

        public Value GetParam(string name)
        {
            if (_cacheValues.ContainsKey(name))
                return _cacheValues[name];

            Value result = this.GetParamValue(name);

            _cacheValues.Add(name, result);

            return result;
        }

        protected Value GetParamValue(string name)
        {
            switch (name)
            {
                case "CF":
                    return _shipment.ContractFactoring;
                case "CD":
                    return _shipment.ContractDelivery;
                case "SH":
                    return _shipment.Title;

                case "Holder":
                    return "Debtor";

                case "StatusRegress":
                    return _shipment.StatusRegress;
                case "StatusFinancing":
                case "StatusFinansing":
                    return _shipment.StatusFinancing;

                case "Nominal":
                    return _shipment.Summ;

                case "MIT.FinComRate":
                    return _shipment.MITComFin;
                case "MIT.NoFinComRate":
                    return _shipment.MITComNoFin;
                case "MIT.MinCom":
                    return _shipment.MITComMin;


                case "CommissionCommonPassing":
                    return _shipment.CommissionCommonPassing;
                case "Shipment.RateCommission":
                case "RateCommission":
                    return _shipment.RateCommission;
                case "PrepayCommission":
                    return _shipment.PrepayCommission;
                case "RateCommissionMin":
                case "Shipment.CommissionBottomBorder":
                    return _shipment.RateCommissionMin;
                
                case "Shipment.RateStandart":
                case "RateStandart":
                    return _shipment.RateStandart;
                case "Shipment.RateExtra":
                case "RateExtra":
                    return _shipment.RateExtra;
              
                case "Shipment.RateDaily":
                case "Shipment.RateValue":
                case "RateValue":
                    return _shipment.RateValue;
                case "RateValueMin":
                    return _shipment.RateValueMin;
                case "RateValuePlus":
                    return _shipment.RateValuePlus;

                //case "Shipment.RatePeny":
                case "RatePeny":
                    return _shipment.RatePeny;
                
                case "WithoutFinancingPercent":
                    return 0;
                case "Delivery.RateAddon":
                    return 0;
                case "Shipment.RateCommissionAddon":
                    return 0;

                case "IsCommissionAccount407":
                    return _shipment.IsAccountCommission407;

                case "IncreasedCommission":
                    return 0;
                case "IncreasedUsed":
                    return 0;

                case "DutyCustomer":
                    return _shipment.Operations.Where(o => o.Type == "CustomerFinansing" || o.Type == "CustomerFinancing").Sum(o => o.Summ);

                case "PayedCommission":
                    return _shipment.Operations.Where(o => o.Type == "CustomerFinansing" || o.Type == "CustomerFinancing" || o.Type.StartsWith("Payment")).Sum(o => o.Commission.Commission);

                case "DateToCustomer":
                    {
                        Func<DateTime, bool> check = d => { return !(d.Hour > 16 || d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday); };

                        DateTime now = _date;

                        while (!check(now))
                            now = now.AddDays(1);

                        return now;
                    }

            }

            throw new ArgumentException(String.Format("Shipment's param {0} not recognized", name));
        }

        public Account GetAccount(string name)
        {
            if (_cacheAccounts.ContainsKey(name))
                return _cacheAccounts[name];

            Account acc = new Account(name, "bik", "metib", "corr");

            _cacheAccounts.Add(name, acc);

            return acc;

            //throw new NotImplementedException("Can't get Shipment's Accounts");
        }

        public RateCommission GetRate(string name)
        {
            bool statusFinancing = _shipment.StatusFinancing;
            bool commonPassing = _shipment.CommissionCommonPassing;

            switch (name)
            {
                // ставка МеталлИнвестТехнологии
                case "MIT":
                    {
                        if (statusFinancing)
                            return new RateCommission(this["MIT.FinComRate"], 0, this["MIT.MinCom"]);
                        else
                            return new RateCommission(this["MIT.NoFinComRate"], 0, this["MIT.MinCom"]);
                    }

                // ставка КП1 (годовые %% от суммы задолженности)
                case "KP1":
                case "Standart":
                case "ResourcesStandart":
                    {
                        if (!statusFinancing || commonPassing)
                            return RateCommission.Zero;

                        double daysCount = _date > this.DateFinancing ? (_date - this.DateFinancing).Days : 0;

                        return new RateCommission(this["RateStandart"] * daysCount / 365.0);
                    }

                // ставка КП1 Экстра (годовые %% от просроченной задолженности)
                case "KP1Extra":
                case "Extra":
                case "ResourcesExtra":
                    {
                        if (commonPassing)
                            return RateCommission.Zero;

                        double daysCount = _date > _shipment.DatePayment ? (_date - _shipment.DatePayment).Days : 0;

                        return new RateCommission(this["RateExtra"] * daysCount / 365);
                    }

                // ставка КП2 (% от суммы накладной)
                case "KP2":
                case "Commission":
                    {
                        if (commonPassing)
                            return RateCommission.Zero;

                        if (!statusFinancing)
                            return new RateCommission(this["WithoutFinancingPercent"], this["Delivery.RateAddon"]);

                        double percent = this["RateCommission"];
                        double addon = this["Shipment.RateCommissionAddon"];
                        double min = this["Shipment.CommissionBottomBorder"];

                        //double rate_any_account = this["Rate_AnyAccount"];
                        //double request_param_any_account = this["RequestParam_AnyAccount"];
                        //double rate_one_day = this["Rate_OneDay"];
                        //double request_param_one_day = this["RequestParam_OneDay"];
                        //double rate_scan = this["Rate_Scan"];
                        //double request_param_scan = this["RequestParam_Scan"];

                        // Подсчет ставки
                        double rate = percent + addon;
                        //double rate = percent + addon +
                        //    rate_any_account * request_param_any_account +
                        //    rate_one_day * request_param_one_day +
                        //    rate_scan * request_param_scan;

                        return new RateCommission(rate, 0, min);
                    }

                // ставка КП3 (% в день от стоимости накладной)
                case "KP3":
                case "Daily":
                    {
                        if (!statusFinancing || commonPassing)
                            return RateCommission.Zero;

                        double daysCount = _date > this.DateFinancing ? (_date - this.DateFinancing).Days : 0;

                        return new RateCommission(this["RateValue"] * daysCount, 0, this["RateValueMin"], this["RateValuePlus"]);
                    }

                // ставка Пени
                case "Peny":
                    {
                        return new RateCommission(this["RatePeny"]);
                    }
            }

            throw new ArgumentException(String.Format("Rate {0} is not recognized for Shipments", name));
        }

    }

}
