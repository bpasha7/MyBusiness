﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext():base("DefaultConnection")
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Material> Materials { get; set; }
        //public DbSet<MyEvent> Events { get; set; }

    }
 
}
