using AccountingFactory.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountingFactory.Factoring
{
    public class ShipmentOperationBuilder : IOperationBuilder
    {
        #region Constants
        public const string CommentDocumentsIncome = "Прием документов, ДФ {CF}, ДП {CD}, поставка {SH}";
        public const string CommentCustomerFinancing = "Выплата финанс-ия по {CF}, ДП {CD}, отгр {SH}, в тч НДС";
        public const string CommentCommissionWriteOut = "[№OrderID] Списание комиссии, ДФ{CF}, ДП{CD}, отгр {SH}, в тч НДС";
        public const string CommentCommission = "[№OrderID] Списание за услуги, ДФ{CF}, ДП{CD}, отгр {SH}";
        public const string CommentIncreasedCommission = "Начисление наращенной комиссии, ДФ{CF}, ДП{CD}, отгр {SH}";
        public const string CommentPayment = "Обработка платежа ДФ{CF}, ДП{CD}, отгр {SH}";
        #endregion

        protected IAccountingParamsProvider _provider;

        public ShipmentOperationBuilder(IAccountingParamsProvider paramsProvider)
        {
            _provider = paramsProvider;
        }

        #region Building Operations
        public IOperation Build(string type, DateTime date, double summ)
        {
            switch (type.Replace(" ", ""))
            {
                case "DocumentsIncome":
                    return this.DocumentsIncome(date, summ);
                case "CustomerFinancing":
                case "CustomerFinansing":
                    return this.CustomerFinancing(date, summ);
                case "PaymentDebtor":
                case "PaymentCustomer":
                case "PaymentGuarantor":
                    return this.Payment(date, summ, type);
                case "IncreasedCommission":
                    return this.IncreasedCommission(date, summ);
            }

            throw new NotImplementedException(String.Format("Shipment Operation {0} is not supported", type));
        }

        private ShipmentOperation DocumentsIncome(DateTime date, Money summ)
        {
            // определяем комиссию МИТ
            var commission = new ShipmentCommission(0, 0, 0, 0, 0, 0, _provider.GetRate("MIT").Calculate(summ));

            // создаем операцию примема документов
            var result = new ShipmentOperation("DocumentsIncome", date, summ, this.MakeComment(CommentDocumentsIncome))
            {
                Commission = commission
            };

            #region Формируем Проводки
            // стандартная проводка покупки задолженности
            this.AddPosting(ref result, "478 03", "474 01");

            // Забалансовый счет. Отражается прием документов в учете банка (учет суммы купленного документа)
            this.AddPosting(ref result, "914 18", "999 99");

            // Забалансовый счет. Отражается учет поручительства (учет суммы документа поручительства)
            if (_provider["StatusRegress"])
                this.AddPosting(ref result, "914 14", "999 99");
            #endregion

            return result;
        }

        private ShipmentOperation CustomerFinancing(DateTime date, Money summ)
        {
            // номинал накладной (сумма накладной с учетом корректировок)
            double nominal = _provider["Nominal"];

            // определяем комиссию в момент финансирования
            var commission = new ShipmentCommission(0, _provider["PrepayCommission"] * _provider.GetRate("KP2").Calculate(nominal), 0, 0, 0);

            // пытаемся использовать наращенную комиссию 
            // обычно такого не бывает, так как финансирум в момент покупки (сразу), когда еще не делали наращивания
            commission.Increased = this.CalculateIncreased();

            // отправлять ли (и списывать соответсвенно) комиссию на 40702
            bool sendCommissionToCustomer40702 = _provider["IsCommissionAccount407"];

            // определяем деньги для поставщика
            var toCustomer = new ShipmentOperationPartToCustomer(_provider["DateToCustomer"], (sendCommissionToCustomer40702 ? summ : summ - commission.Total).NotLess(0));

            // создаем операцию выплаты финансирования
            var result = new ShipmentOperation("CustomerFinansing", date, summ, this.MakeComment(CommentCustomerFinancing))
            {
                Commission = commission,
                ToCustomer = toCustomer
            };

            #region Формируем проводки
            if (result.Commission.Total > 0)
            {
                // комментарий для комиссионных проводок
                string comment = this.MakeComment(CommentCommission);

                // комиссионные проводки
                if (sendCommissionToCustomer40702)
                {
                    // если мы зачислили финансирование И комиссию на р/с
                    // то нужно теперь комиссию списывать с р/с
                    this.AddPosting(ref result, "40", "474 23", result.Commission.Total, this.MakeComment(CommentCommissionWriteOut), "WriteOut");
                }
                else
                {
                    // если мы НЕ зачисляли комиссию на р/с,
                    // то ее нужно списывать (рреализовывать) сразу с 47401

                    //списание (погашение) донарощенной (только что) комиссии
                    this.AddPosting(ref result, "474 01", "474 23", result.Commission.IncreseNeeded, comment, "Commission");

                    //списание (погашение) уже нарощенной комиссии ранее (скорее всего такой проводки НЕ должно быть, так как еще не было наращивания)
                    this.AddPosting(ref result, "474 01", "474 23", result.Commission.Increased, comment, "Commission");

                    //списание (погашение) НДС
                    this.AddPosting(ref result, "474 01", "474 23", result.Commission.TotalNDS, comment, "Standart"); 
                }

                // донаращивание комиссии (зачисление комиссии в прибыль)
                this.AddPosting(ref result, "474 23", "706 01", result.Commission.IncreseNeeded, comment, "Standart"); 

                // зачисление в НДС
                this.AddPosting(ref result, "474 23", "603", result.Commission.TotalNDS, comment, "Commission"); 
            }

            // выплата финансирования
            this.AddPosting(ref result, "474 01", "40",  result.ToCustomer.Date, result.ToCustomer.Send, result.Comment, "Payment");

            // зачитывание финансирования (если есть)
            if (result.ToCustomer.Netting.Summ > 0)
                this.AddPosting(ref result, "474 01", result.ToCustomer.Netting.Account.Number, result.ToCustomer.Date, result.ToCustomer.Netting.Summ, result.Comment, "Standart");
            #endregion

            return result;
        }

        private ShipmentOperation IncreasedCommission(DateTime date, Money summ)
        {
            // определяем комиссию в момент финансирования
            var commission = this.CalculateCommission(summ);

            // создаем операцию наращивания
            var result = new ShipmentOperation("IncreasedCommission", date, summ, this.MakeComment(CommentIncreasedCommission))
            {
                Commission = commission
            };

            #region Формируем проводки
            // наращивание комиссии в прибыль
            this.AddPosting(ref result, "474 23", "706 01", result.Commission.IncreseNeeded);
            #endregion

            return result;
        }

        private ShipmentOperation Payment(DateTime date, Money summ, string type = "Payment Debtor")
        {
            // определяем комиссию по платежу
            var commission = this.CalculateCommission(summ);

            // определяем деньги для поставщика
            var toCustomer = new ShipmentOperationPartToCustomer(_provider["DateToCustomer"], (summ - commission.Total - commission.Body).NotLess(0));

            // создаем операцию оплаты
            var result = new ShipmentOperation(type, date, summ, this.MakeComment(CommentPayment))
            {
                Commission = commission,
                ToCustomer = toCustomer
            };

            #region Формируем проводки
            // погашение долга дебитора 
            string holder = _provider["Holder"];
            if (holder.Equals("debtor", StringComparison.InvariantCultureIgnoreCase))
                this.AddPosting(ref result, "612 12", "478 03");
            else if (holder.Equals("customer", StringComparison.InvariantCultureIgnoreCase))
                this.AddPosting(ref result, "612 12", "474 23");

            // погашение внебалансовых документов (номинала накладных)
            this.AddPosting(ref result, "999 99", "914 18");

            // погашение внебалансовых поручительств
            if (_provider["StatusRegress"])
                this.AddPosting(ref result, "999 99", "914 14");

            // комиссионные проводки
            if (result.Commission.Total > 0)
            {
                // комментарий для комиссионных проводок
                string comment = this.MakeComment(CommentCommission);

                // донаращивание комиссии (зачисление комиссии в прибыль)
                this.AddPosting(ref result, "474 23", "706 01", result.Date, result.Commission.IncreseNeeded, comment, "Standart");

                //списание (погашение) донарощенной (только что) комиссии
                this.AddPosting(ref result, "474 01", "474 23", result.Date, result.Commission.IncreseNeeded, comment, "Commission"); 

                //списание (погашение) уже нарощенной комиссии
                this.AddPosting(ref result, "474 01", "474 23", result.Date, result.Commission.Increased, comment, "Commission"); 

                //списание (погашение) НДС
                this.AddPosting(ref result, "474 01", "474 23", result.Date, result.Commission.TotalNDS, comment, "Standart"); 
                
                // зачисление в НДС
                this.AddPosting(ref result, "474 23", "603", result.Date, result.Commission.TotalNDS, comment, "Commission"); 
            }

            // выплата остатка
            this.AddPosting(ref result, "474 01", "40", result.ToCustomer.Date, result.ToCustomer.Send, result.Comment, "Payment");

            // зачитывание остатка (если есть)
            if (result.ToCustomer.Netting.Summ > 0)
                this.AddPosting(ref result, "474 01", result.ToCustomer.Netting.Account.Number, result.ToCustomer.Date, result.ToCustomer.Netting.Summ, result.Comment, "Standart");
            #endregion

            return result;
        }
        #endregion

        #region Functions
        protected string MakeComment(string comment)
        {
            return Regex.Replace(comment, @"\{(?<name>[\w\. -]+)}", m => _provider.GetParam(m.Groups["name"].Value).ToString(), RegexOptions.Compiled);
        }

        protected Posting AddPosting(ref ShipmentOperation operation, string debt, string cred, DateTime date, Money summ, string comment = "", string type = "Standart")
        {
            var accDebt = _provider.GetAccount(debt);
            var accCred = _provider.GetAccount(cred);

            var posting = new Posting(type, accDebt, accCred, date, summ, String.IsNullOrEmpty(comment) ? operation.Comment : comment);

            operation.AddPosting(posting);

            return posting;
        }

        protected Posting AddPosting(ref ShipmentOperation operation, string debt, string cred, Money summ, string comment = "", string type = "Standart")
        { return this.AddPosting(ref operation, debt, cred, operation.Date, summ, comment, type); }

        protected Posting AddPosting(ref ShipmentOperation operation, string debt, string cred)
        { return this.AddPosting(ref operation, debt, cred, operation.Date, operation.Summ, operation.Comment, "Standart"); }

        /// <summary>
        /// Расчет комисии от суммы пришедшего платежа
        /// </summary>
        /// <param name="summ">Сумма поступившего платежа</param>
        /// <returns>Расчитанная Комиссия за платеж</returns>
        private ShipmentCommission CalculateCommission(Money summ, bool increased = true)
        {
            // определяем текущую сумму долга поставщика (остаток тела кредита)
            double dutyCutomer = _provider["DutyCustomer"];

            // номинал накладной
            double nominal = _provider["Nominal"];

            // определяем значения ставок
            RateCommission rateStandart = _provider.GetRate("KP1");
            RateCommission rateExtra = _provider.GetRate("KP1Extra");
            RateCommission rateCommission = _provider.GetRate("KP2");
            RateCommission rateDaily = _provider.GetRate("KP3");
            RateCommission ratePeny = _provider.GetRate("Peny");

            // определяем уже оплаченную комиссию КП2 по накладной до настоящей операции
            double payedCommission = _provider["PayedCommission"];

            // остаток от плататежа
            Money rest = summ;

            //рассчет комисии КП2 всегда зависит от номинала накладной
            Money commission = (rateCommission.Calculate(nominal) - payedCommission).NotMore(rest).NotLess(0);
            rest -= commission;

            //рассчет суммы Пени от суммы платежа
            Money peny = ratePeny.Calculate(summ).NotMore(rest).NotLess(0);
            rest -= peny;

            //рассчет комисии КП3 от суммы платежа
            Money daily = rateDaily.Calculate(summ).NotMore(rest).NotLess(0);
            rest -= daily;

            // определяем ту часть тела кредита, которую можем погасить (не более остатка самого тела)
            Money body = (rest / (1 + rateStandart.Rate + rateExtra.Rate)).NotMore(dutyCutomer).NotMore(rest).NotLess(0);
            rest -= body;

            // на основании погашаемой части тела можем расчитать годовую комиссию стандарт
            Money standart = rateStandart.Calculate(body).NotMore(rest).NotLess(0);
            rest -= standart;

            // на основании погашаемой части тела можем расчитать годовую комиссию за просрочку экстра
            Money extra = new Money(rateExtra.Calculate(body)).NotMore(rest).NotLess(0);
            rest -= extra;

            // создаем комиссионную структуру
            var result = new ShipmentCommission(body, commission, standart, extra, daily, peny);

            // если необходимо, учитываем наращенную комиссию
            if (increased)
                result.Increased = this.CalculateIncreased();

            return result;
        }

        /// <summary>
        /// Подсчет "УЖЕ НАРАЩЕННОЙ РАНЕЕ" комиссии, ДОСТУПНОЙ для списания (использования) на данный момент
        /// </summary>
        /// <remarks>
        /// Все дело в том, что часть комиссии мы наращиваем в конце месяца (показываем эту комиссию на 706).
        /// Поэтому в момент обработки (который после наращивания) сумма комиссии уже больше чем в момент наращивания (так как прошло лишнее время с момента наращивания)
        /// Поэтому нам нужно еще донаростить комиссию, то есть добавить недостающие проводки на 706 (так как некоторые уже были проведены в конце месяца) 
        /// Вопрос - сколько нужно донаростить на 706? 
        /// Ответ - сумма желаемой комиссии минус столько, сколько наращивали ранее.
        /// </remarks>
        /// <example>
        /// Например, с помощью оперций наращивания (IncreasedCommission) мы нарастили 100 рублей (47423-706).
        /// При следующей операции списания комиссии (например, PaymentDebtor) мы насчитали чистыми 180р комисии.
        /// Так как ранее мы нарастили 100р, то теперь осталось нарастить 80р (47423-706=80р), чтобы реализовать всю комисиию данной операции 
        /// остальные проводки по реализации комиссии (47401-47423 || 40702-47423) мы будем писать на все 180р.
        /// </example>
        /// <returns>Сумма УЖЕ наращенной РАНЕЕ комиссии, которую МОЖНО использовать в данный момент</returns>
        private Money CalculateIncreased()
        {
            // столько комиссии было "наращено" с помощью операций наращивания ("IncreasedCommission")
            double increased = _provider["IncreasedCommission"];

            // столько комиссии было "списано" с наращивания с помощью операций "обработки" и др, где есть комиссия для списания с наращивания
            double used = _provider["IncreasedUsed"];

            // столько комиссии из "наращенной" мы можем использовать в данный момент (остаток наращенной комиссии)
            // то есть ровно эту часть НЕ нужно больше наращивать, так как мы ее нарастили ранее!!!
            return new Money(increased - used).NotLess(0);
        }
        #endregion
    }
}
