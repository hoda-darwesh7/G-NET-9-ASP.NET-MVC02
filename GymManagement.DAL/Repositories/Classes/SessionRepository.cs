using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository 
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository( GymDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<int> CountOfBookedSlotsAsync(int id, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.AsNoTracking().CountAsync(B =>B.SessionId == id);
        }


        public async Task<IEnumerable<Session>> GetSessionsWithTrainerAndCategory(CancellationToken ct = default)
        {
            var query = _dbContext.Sessions.AsNoTracking().Include(t => t.Trainer).Include(c => c.Category);
            return await query.ToListAsync();
        }
    }
}
