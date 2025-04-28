using System;
using Transport_Management;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Transport_Management.Models.Entity;

namespace Transport_Management.Helpers.DbContexts
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options) { }
        public DbSet<mstUser> mstUsers { get; set; }
        public DbSet<mstUserType> mstUserType { get; set; }
        public DbSet<mstVehicle> mstVehicle { get; set; }
        public DbSet<mstCompany> mstCompany { get; set; }
        public DbSet<mstRentalEntry> mstRentalEntry { get; set; }
        public DbSet<mstInvoice> mstInvoice { get; set; }


    }
}
