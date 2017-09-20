using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class MyEvent
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeFinish{ get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string Commentary { get; set; }
        public decimal Payment { get; set; }
        public decimal Prepayment { get; set; }
        public string Name { get; set; }
        public string Items { get; set; }

    }
}
