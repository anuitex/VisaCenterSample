using Microsoft.EntityFrameworkCore;
using System;
using VisaCenter.Repository.Models;

namespace VisaCenter.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Visa> Visas { get; set; }

        public ApplicationDbContext() : this(new DbContextOptions<ApplicationDbContext>())
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
