
using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.DbContexts

{
    public class HRMSContext : DbContext
    {
      public HRMSContext (DbContextOptions<HRMSContext> options) : base(options) 
        {

        }

        // seeding the database by using this function 
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {

            base.OnModelCreating(modelBuilder); // we have to call the parent function to avoid errors 

            modelBuilder.Entity<Lookup>().HasData( // this function is responsible to seed the database 

                 // Employee Positions (Major code = 0)
                  new Lookup { Id = 1, MajorCode = 0 , MinorCode = 0 , Name = "Employee Positions"},

                    new Lookup { Id = 2, MajorCode = 0 ,MinorCode = 1, Name = " Developer" },
                    new Lookup { Id = 3, MajorCode = 0, MinorCode = 2, Name = " HR" },
                    new Lookup { Id = 4, MajorCode = 0, MinorCode = 3, Name = "Manager " },

                  // Department Types (Major Code = 1 )
                  new Lookup { Id = 5, MajorCode = 1, MinorCode = 0, Name = "Department Types" },

                     new Lookup { Id = 6, MajorCode = 1, MinorCode = 1, Name = "Finance " },
                     new Lookup { Id = 7, MajorCode = 1, MinorCode = 2, Name = "Administrative" },
                     new Lookup { Id = 8, MajorCode = 1, MinorCode = 3, Name = "Technical" },

                  // Vacation Types (Major code = 2 )
                     new Lookup { Id = 9, MajorCode = 2, MinorCode = 0, Name = "Vacation Types " },

                       new Lookup { Id = 10, MajorCode = 2, MinorCode = 1, Name = "Sick leave " },
                       new Lookup { Id = 11, MajorCode = 2, MinorCode = 2, Name = "Annual Leave" },
                       new Lookup { Id = 12, MajorCode = 2, MinorCode = 3, Name = "Unpaid Leave"}
                );
        }




        // Tables 
        public DbSet<Employee> Employees { get; set; }
        public DbSet <Department> Departments { get; set; }
        public DbSet <Lookup> Lookups { get; set; }
    }
}
