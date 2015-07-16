using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    public struct Account
    {
        private string _number;
        private string _bik;
        private string _bank;
        private string _correspondent;

        public string Number
        { get { return _number; } }

        public string BIK
        { get { return _bik; } }

        public string Bank
        { get { return _bank; } }

        public string Correspondent
        { get { return _correspondent; } }

        public Account(string number, string bik, string bank, string correspondent)
        {
            _number = number;
            _bik = bik;
            _bank = bank;
            _correspondent = correspondent;
        }

        public override string ToString()
        {
            return this.Number;
        }
    }
}
