using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class Order : DbEntitie
    {
        //public int Id { get; set; }
        public int ClientId { get; set; }
        public int PriceId { get; set; }
        public decimal Payment { set; get; }
        public DateTime Date { get; set; }
        public string CalendarId { get; set; }
        public bool Left { get; set; }
    }
}
