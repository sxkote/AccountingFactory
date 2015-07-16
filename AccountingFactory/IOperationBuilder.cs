using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    public interface IOperationBuilder
    {
        IOperation Build(string type, DateTime date, double summ);
    }
}
