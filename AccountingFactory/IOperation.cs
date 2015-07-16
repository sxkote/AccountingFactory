using System;
using System.Collections.Generic;
namespace AccountingFactory
{
    public interface IOperation
    {
        string Type { get; }

        DateTime Date { get; }
        
        Money Summ { get; }
        
        string Comment { get; }

        ICollection<Posting> Postings { get; }
    }
}
