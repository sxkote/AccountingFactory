using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Factoring
{
    public struct FactoringCommissionParams
    {
        private bool _commonPassing;
        private bool _statusFinancing;

        private RateCommission _rateWithoutFinancing;
        private RateCommission _rateFinancing;

        public bool CommonPassing
        { get { return _commonPassing; } }

        public bool StatusFinancing
        { get { return _statusFinancing; } }

        public RateCommission RateWithoutFinancing
        { get { return _rateWithoutFinancing; } }

        public RateCommission RateFinancing
        { get { return _rateFinancing; } }

        public FactoringCommissionParams(bool commonPassing, bool statusFinancing, RateCommission rateWithoutFinancing, RateCommission rateFinancing)
        {
            _commonPassing = commonPassing;
            _statusFinancing = statusFinancing;
            _rateWithoutFinancing = rateWithoutFinancing;
            _rateFinancing = rateFinancing;
        }
    }
}
