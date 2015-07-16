using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    public class Operation : IOperation
    {
        protected string _type;
        protected DateTime _date;
        protected Money _summ;
        protected string _comment;
        protected ICollection<Posting> _postings;

        public string Type
        { get { return _type; } }

        public DateTime Date
        { get { return _date; } }

        public Money Summ
        { get { return _summ; } }

        public string Comment
        { get { return _comment; } }

        public ICollection<Posting> Postings
        { get { return _postings.ToList().AsReadOnly(); } }

        public Operation(string type, DateTime date, Money summ, string comment = "")
        {
            _type = type;
            _date = date;
            _summ = summ;
            _comment = comment;
            _postings = new List<Posting>();
        }

        public Posting AddPosting(Posting posting)
        {
            _postings.Add(posting);
            return posting;
        }
    }
}
