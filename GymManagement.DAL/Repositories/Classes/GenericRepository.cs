using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _Set;

        public GenericRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _Set = dbContext.Set<TEntity>();
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            _Set.Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return _Set.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            _Set.Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking? _Set : _Set.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
            =>await _Set.FindAsync(id, ct);

        public async Task<int> UpdateAsync(TEntity entity)
        {
            _Set.Update(entity);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
