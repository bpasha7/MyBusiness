using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class OrdersFullReport
    {
    }

    public class OrderInfo
    {
        public string ClientName { get; set; }
        public string PriceName { get; set; }
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
    }
}
