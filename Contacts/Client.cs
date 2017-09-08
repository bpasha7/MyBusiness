using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { set; get; }
        public string Phone { get; set; }
        public string Link { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
    }
}
