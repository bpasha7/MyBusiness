using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class ReportByMonth
    {
        public DateTime Date { get; set; }
        public decimal OrdersSum { get; set; }
        public decimal MaterialsSum { get; set; }
        public decimal Dif { get { return OrdersSum - MaterialsSum; } }
    }
}
