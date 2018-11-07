using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VisaCenter.Repository.Interfaces;
using VisaCenter.Repository.Models;

namespace VisaCenter.Repository.Repositories
{
    public class VisaRepository : RepositoryBase<Visa>, IVisaRepository
    {
        public VisaRepository(ApplicationDbContext dbContext, IServiceScopeFactory scopeFactory) : base(dbContext, scopeFactory)
        {
        }

        protected override DbSet<Visa> DbSet => DbContext.Visas;
    }

    public interface IVisaRepository : IRepository<Visa>
    {

    }
}
