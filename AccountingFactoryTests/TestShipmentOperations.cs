using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountingFactory.Factoring;
using AccountingFactory;

namespace AccountingFactoryTests
{
    [TestClass]
    public class TestShipmentOperations
    {
        public static DateTime ShipmentDateShipment = new DateTime(2015, 3, 1);
        public static DateTime ShipmentDatePayment = new DateTime(2015, 3, 31);

        [TestInitialize]
        public void Initialize()
        {
        }

        protected ExampleShipment GenerateShipment(Money summ, bool regress = true, bool financing = true, double rateFinancing = 0.8, double rateCom = 0, double rateComMin = 0, double rateStandart = 0, double rateExtra = 0.7, double rateValue = 0, double rateValueMin = 0, bool isCommissionAccount407 = false)
        {
            return new ExampleShipment()
            {
                ContractFactoring = "FCT",
                ContractDelivery = "DLV",
                Title = "SHIPMENT",
                DateShipment = ShipmentDateShipment,
                DatePayment = ShipmentDatePayment,
                Summ = summ,
                StatusRegress = regress,
                StatusFinancing = financing,
                RateFinancing = rateFinancing,
                MITComFin = 0,
                MITComMin = 0,
                MITComNoFin = 0,
                CommissionCommonPassing = false,
                PrepayCommission = 0,
                RateCommission = rateCom,
                RateCommissionMin = rateComMin,

                RateStandart = rateStandart,
                RateExtra = rateExtra,
                RateValue = rateValue,
                RateValueMin = rateValueMin,
                RateValuePlus = 0,
                RatePeny = 0,

                IsAccountCommission407 = isCommissionAccount407
            };
        }

        protected ShipmentOperationBuilder GenerateBuilder(ExampleShipment shipment, DateTime date, double summ, string type="")
        {
            var provider = new ExampleShipmentParamsProvider(shipment, date, summ, type);
            return new ShipmentOperationBuilder(provider);
        }

        [TestMethod]
        public void build_DocumentsIncome_regress()
        {
            var shipment = this.GenerateShipment(10000, regress: true);

            AssertDocumentsIncome(shipment);
        }

        [TestMethod]
        public void build_DocumentsIncome_noregress()
        {
            var shipment = this.GenerateShipment(10000, regress: false);

            AssertDocumentsIncome(shipment);
        }

        [TestMethod]
        public void build_DocumentsIncome_mit()
        {
            var mitComMin = 118;
            var mitComFin = 0.01;
            var mitComNoFin = 0.0001;

            var shipment = this.GenerateShipment(10000, financing: true);
            shipment.MITComFin = mitComFin;
            shipment.MITComNoFin = mitComNoFin;
            shipment.MITComMin = mitComMin;
            AssertDocumentsIncome(shipment);

            shipment = this.GenerateShipment(8439323, financing: true);
            shipment.MITComFin = mitComFin;
            shipment.MITComNoFin = mitComNoFin;
            shipment.MITComMin = mitComMin;
            AssertDocumentsIncome(shipment);

            shipment = this.GenerateShipment(364823764, financing: false);
            shipment.MITComFin = mitComFin;
            shipment.MITComNoFin = mitComNoFin;
            shipment.MITComMin = mitComMin;
            AssertDocumentsIncome(shipment);
        }

        public ShipmentOperation AssertDocumentsIncome(ExampleShipment shipment)
        {
            DateTime date = ShipmentDateShipment;

            var builder = this.GenerateBuilder(shipment, date, shipment.Summ, "DocumentsIncome");

            var operation = builder.Build("DocumentsIncome", date, shipment.Summ) as ShipmentOperation;
            Assert.AreEqual(shipment.Summ, operation.Summ.Value);
            Assert.AreEqual(date, operation.Date);
            Assert.AreEqual("DocumentsIncome", operation.Type);

            //не должно быть комисии
            Assert.AreEqual(0, operation.Commission.Total);
            Assert.AreEqual(new Money(shipment.Summ * ((shipment.StatusFinancing) ? shipment.MITComFin : shipment.MITComNoFin)).NotLess(shipment.MITComMin), operation.Commission.MIT);
            Assert.AreEqual(0, operation.Commission.Body);
            Assert.AreEqual(0, operation.Commission.Increased);

            Assert.AreEqual(0, operation.ToCustomer.Summ);
            Assert.AreEqual(0, operation.ToCustomer.Netting.Summ);

            Assert.AreEqual(shipment.StatusRegress ? 3 : 2, operation.Postings.Count);
            Assert.IsTrue(operation.Postings.All(p => p.Summ == shipment.Summ));
            Assert.IsTrue(operation.Postings.All(p => p.Date == date));
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("478 03") && p.Cred.Number.StartsWith("474 01")));
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("914 18") && p.Cred.Number.StartsWith("999 99") && p.Summ == shipment.Summ));
            if (shipment.StatusRegress)
                Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("914 14") && p.Cred.Number.StartsWith("999 99") && p.Summ == shipment.Summ));

            return operation;
        }

        [TestMethod]
        public void build_CustomerFinancing_without_commission()
        {
            var shipment = this.GenerateShipment(10000, rateCom: 0.01, rateComMin: 118);
            shipment.PrepayCommission = 0;

            AssertCustomerFinancing(shipment);
        }

        [TestMethod]
        public void build_CustomerFinancing_with_commission_prepay_nocomacc407()
        {
            var shipment = this.GenerateShipment(10000, rateCom: 0.01, rateComMin: 118);
            shipment.PrepayCommission = 1;

            AssertCustomerFinancing(shipment);
        }

        [TestMethod]
        public void build_CustomerFinancing_with_commission_prepay_comacc407()
        {
            var shipment = this.GenerateShipment(10000, rateCom: 0.01, rateComMin: 118, isCommissionAccount407: true);
            shipment.PrepayCommission = 1;

            AssertCustomerFinancing(shipment);
        }

        public ShipmentOperation AssertCustomerFinancing(ExampleShipment shipment)
        {
            DateTime date = ShipmentDateShipment;

            Money summ = shipment.Summ;
            Money financing = Math.Round(shipment.Summ * shipment.RateFinancing, 2);

            var builder = this.GenerateBuilder(shipment, date, financing, "CustomerFinansing");

            var operation = builder.Build("CustomerFinancing", date, shipment.Summ * shipment.RateFinancing) as ShipmentOperation;
            Assert.AreEqual(financing, operation.Summ.Value);
            Assert.AreEqual(date, operation.Date);
            Assert.AreEqual("CustomerFinansing", operation.Type);

            //не должно быть комисии
            Assert.AreEqual(Math.Round(shipment.PrepayCommission * Math.Max(shipment.Summ * shipment.RateCommission, shipment.RateCommissionMin), 2), operation.Commission.Total.Value);
            Assert.AreEqual(0, operation.Commission.MIT);
            Assert.AreEqual(0, operation.Commission.Body);
            Assert.AreEqual(0, operation.Commission.Increased);

            Assert.AreEqual(shipment.IsAccountCommission407 ? financing : financing - operation.Commission.Total, operation.ToCustomer.Summ);
            Assert.AreEqual(0, operation.ToCustomer.Netting.Summ);

            Assert.AreEqual((operation.ToCustomer.Netting.Summ > 0 ? 2 : 1) + (shipment.PrepayCommission > 0 ? (shipment.IsAccountCommission407 ? 3 : 5) : 0), operation.Postings.Count);
            //Assert.IsTrue(operation.Postings.All(p => p.Summ == shipment.Summ));
            //Assert.IsTrue(operation.Postings.All(p => p.Date.Date == date.Date));
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("474 01") && p.Cred.Number.StartsWith("40") && p.Summ == (shipment.IsAccountCommission407 ? financing : financing - operation.Commission.Total)));
            if (shipment.PrepayCommission > 0)
            {
                if (shipment.IsAccountCommission407)
                    Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("40") && p.Cred.Number.StartsWith("474 23")));
                else
                    Assert.IsTrue(operation.Postings.Count(p => p.Debt.Number.StartsWith("474 01") && p.Cred.Number.StartsWith("474 23")) == 3);

                Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("474 23") && p.Cred.Number.StartsWith("603") && p.Summ == operation.Commission.TotalNDS));
                Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("474 23") && p.Cred.Number.StartsWith("706") && p.Summ == operation.Commission.TotalClear));
            }

            return operation;
        }

        [TestMethod]
        public void build_Payment_commission_min()
        {
            var shipment = this.GenerateShipment(10000, rateCom: 0.01, rateComMin: 118);
            shipment.PrepayCommission = 0;

            shipment.Operations.Add(this.AssertDocumentsIncome(shipment));
            shipment.Operations.Add(this.AssertCustomerFinancing(shipment));

            var operation = AssertPayment(shipment, ShipmentDatePayment);

            Assert.AreEqual(118, operation.Commission.Total.Value);
            Assert.AreEqual(118, operation.Commission.Commission.Value);
            Assert.AreEqual(1882, operation.ToCustomer.Send.Value);
        }

        [TestMethod]
        public void build_Payment_commission()
        {
            var shipment = this.GenerateShipment(100000, rateCom: 0.01, rateComMin: 118);
            shipment.PrepayCommission = 0;

            shipment.Operations.Add(this.AssertDocumentsIncome(shipment));
            shipment.Operations.Add(this.AssertCustomerFinancing(shipment));

            var operation = AssertPayment(shipment, ShipmentDatePayment);

            Assert.AreEqual(1000, operation.Commission.Total.Value);
            Assert.AreEqual(1000, operation.Commission.Commission.Value);
            Assert.AreEqual(19000, operation.ToCustomer.Send.Value);
        }

        [TestMethod]
        public void build_Payment_commission_with_prepay()
        {
            var shipment = this.GenerateShipment(100000, rateCom: 0.01, rateComMin: 118);
            shipment.PrepayCommission = 0.4;

            shipment.Operations.Add(this.AssertDocumentsIncome(shipment));
            shipment.Operations.Add(this.AssertCustomerFinancing(shipment));

            var operation = AssertPayment(shipment, ShipmentDatePayment);

            Assert.AreEqual(600, operation.Commission.Total.Value);
            Assert.AreEqual(600, operation.Commission.Commission.Value);
            Assert.AreEqual(19400, operation.ToCustomer.Send.Value);
        }

        [TestMethod]
        public void build_Payment_standart()
        {
            var shipment = this.GenerateShipment(100000, rateStandart: 0.10);
            shipment.PrepayCommission = 0;

            shipment.Operations.Add(this.AssertDocumentsIncome(shipment));
            shipment.Operations.Add(this.AssertCustomerFinancing(shipment));

            var operation = AssertPayment(shipment, ShipmentDatePayment);

            Assert.AreEqual(657.53, operation.Commission.Total.Value);
            Assert.AreEqual(657.53, operation.Commission.Standart.Value);
            Assert.AreEqual(19342.47, operation.ToCustomer.Send.Value);
        }


        public ShipmentOperation AssertPayment(ExampleShipment shipment, DateTime date)
        {
            var builder = this.GenerateBuilder(shipment, date, shipment.Summ, "PaymentDebtor");

            var operation = builder.Build("Payment Debtor", date, shipment.Summ) as ShipmentOperation;
            Assert.AreEqual(shipment.Summ, operation.Summ.Value);
            Assert.AreEqual(date, operation.Date);
            Assert.AreEqual("Payment Debtor", operation.Type);

            //не должно быть комисии
            Assert.IsTrue(operation.Commission.Total > 0);
            Assert.AreEqual(0, operation.Commission.MIT);
            Assert.AreEqual(shipment.DutyCustomer, operation.Commission.Body.Value);
            Assert.AreEqual(0, operation.Commission.Increased);

            Assert.AreEqual(shipment.Summ - shipment.DutyCustomer - operation.Commission.Total, operation.ToCustomer.Summ.Value);
            Assert.AreEqual(0, operation.ToCustomer.Netting.Summ);

            Assert.AreEqual(shipment.StatusRegress ? 9 : 8, operation.Postings.Count);
            //Assert.IsTrue(operation.Postings.All(p => p.Summ == shipment.Summ));
            Assert.IsTrue(operation.Postings.All(p => p.Date.Date == date.Date));

            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("612 12") && p.Cred.Number.StartsWith("478 03") && p.Summ == shipment.Summ));
            Assert.IsTrue(operation.Postings.Count(p => p.Debt.Number.StartsWith("474 01") && p.Cred.Number.StartsWith("474 23")) == 3);
            Assert.IsTrue(operation.Postings.Sum(p => (p.Debt.Number.StartsWith("474 01") && p.Cred.Number.StartsWith("474 23")) ? p.Summ : 0) == operation.Commission.Total);
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("474 23") && p.Cred.Number.StartsWith("603") && p.Summ == operation.Commission.TotalNDS));
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("474 23") && p.Cred.Number.StartsWith("706") && p.Summ == operation.Commission.TotalClear));
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("474 01") && p.Cred.Number.StartsWith("40") && p.Summ == operation.ToCustomer.Send));
            Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("999 99") && p.Cred.Number.StartsWith("914 18") && p.Summ == shipment.Summ));
            if (shipment.StatusRegress)
                Assert.IsTrue(operation.Postings.Any(p => p.Debt.Number.StartsWith("999 99") && p.Cred.Number.StartsWith("914 14") && p.Summ == shipment.Summ));

            return operation;
        }
    }
}
