using AccountingFactory.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    public interface IAccountingParamsProvider
    {
        Value this[string name] { get; }
        Value GetParam(string name);
        Account GetAccount(string name);
        RateCommission GetRate(string name);
    }
}
