using AccountingFactory.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingFactory.Factoring;

namespace AccountingFactory
{
    public interface IAccountingParamsProvider
    {
        Value this[CachePack val] { get; }
        Value GetParam(CachePack val);
        Account GetAccount(string name);
        RateCommission GetRate(CachePack val);
    }
}
