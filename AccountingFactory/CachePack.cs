using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingFactory
{
    public struct CachePack
    {
        private string name;
        private DateTime date;

        public string Name { get { return name; } }
        public DateTime Date { get { return date; } }

        public CachePack(string name, DateTime date)
        {
            this.name = name;
            this.date = date;
        }
        public CachePack(string name)
        {
            this.name = name;
            this.date = DateTime.Now;
        }


        static public implicit operator string(CachePack val)
        { return val.Name; }

        static public implicit operator CachePack(string name)
        { return new CachePack(name); }
    }
}
