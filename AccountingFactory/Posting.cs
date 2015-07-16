using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    public struct Posting
    {
        private string _type;
        private Account _debt;
        private Account _cred;
        private DateTime _date;
        private Money _summ;
        private string _comment;

        public string Type
        { get { return _type; } }

        public Account Debt
        { get { return _debt; } }

        public Account Cred
        { get { return _cred; } }

        public DateTime Date
        { get { return _date; } }

        public Money Summ
        { get { return _summ; } }

        public string Comment
        { get { return _comment; } }

        public Posting(string type, Account debt, Account cred, DateTime date, Money summ, string comment)
        {
            _type = type;
            _debt = debt;
            _cred = cred;
            _date = date;
            _summ = summ;
            _comment = comment;
        }

        public override string ToString()
        {
            return String.Format("[{1}]: {2} -> {3} = {4}: [{0}] '{5}'", this.Type, this.Date.ToShortDateString(), this.Debt, this.Cred, this.Summ, this.Comment);
        }
    }
}
