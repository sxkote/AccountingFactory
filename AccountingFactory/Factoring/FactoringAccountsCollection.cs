using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory.Factoring
{
    public struct FactoringAccountsCollection
    {
        private Account _account91414;
        private Account _account91418;
        private Account _account99999;
        private Account _account47803;
        private Account _account47401;
        private Account _account61212;
        private Account _account47423;
        private Account _account40;

        public Account Acc91414 { get { return _account91414; } }
        public Account Acc91418 { get { return _account91418; } }
        public Account Acc99999 { get { return _account99999; } }
        public Account Acc47803 { get { return _account47803; } }
        public Account Acc47401 { get { return _account47401; } }
        public Account Acc61212 { get { return _account61212; } }
        public Account Acc47423 { get { return _account47423; } }
        public Account Acc40 { get { return _account40; } }


        public FactoringAccountsCollection(IAccountingParamsProvider paramsProvider)
        {
            _account91414 = paramsProvider.GetAccount("914 14");
            _account91418 = paramsProvider.GetAccount("914 18");
            _account99999 = paramsProvider.GetAccount("999 99");
            _account47803 = paramsProvider.GetAccount("478 03");
            _account47401 = paramsProvider.GetAccount("474 01");
            _account61212 = paramsProvider.GetAccount("612 12");
            _account47423 = paramsProvider.GetAccount("474 23");
            _account40 = paramsProvider.GetAccount("40");
        }
    }
}
