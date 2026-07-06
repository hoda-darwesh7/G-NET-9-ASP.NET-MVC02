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
    public class MembershipRepository : GenericRepository<MemberShip>, IMembershipRepository
    {
        private readonly GymDbContext _dbContext;

        public MembershipRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MemberShip>> GetMembershipsWithMemberAndPlanAsync(Expression<Func<MemberShip, bool>>? expression, CancellationToken ct = default)
        {
            var query = _dbContext.MemberShips.Include(x => x.Member).Include(x=>x.Plan).AsNoTracking();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return await query.ToListAsync(ct);
        }
    }
}
