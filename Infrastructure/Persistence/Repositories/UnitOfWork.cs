using DomainLayer.Contracts;
using DomainLayer.Models;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UnitOfWork(BloodDonationDbContext _dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, Object> _repositories = [];
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var Type = typeof(TEntity).Name;
            if (_repositories.TryGetValue(Type, out object? Value))
            {
                return (IGenericRepository<TEntity, TKey>)Value;
            }
            else
            {
                var Repo = new GenericRepository<TKey, TEntity>(_dbContext);
                _repositories.Add(Type, Repo);
                return Repo;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
