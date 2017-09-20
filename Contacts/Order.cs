using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int Price { set; get; }
        public DateTime Date { get; set; }
        public string CalendarId { get; set; }
        public bool Left { get; set; }
    }
}
