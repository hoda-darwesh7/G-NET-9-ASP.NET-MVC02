using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces;

public interface IMembershipRepository :IGenericRepository<MemberShip>
{
    Task<IEnumerable<MemberShip>> GetMembershipsWithMemberAndPlanAsync(Expression<Func<MemberShip, bool>>? expression, CancellationToken ct = default);
}
