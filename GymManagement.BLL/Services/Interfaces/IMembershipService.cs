using GymManagement.BLL.VeiwModels.Common;
using GymManagement.BLL.VeiwModels.MembershipsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipsViewModel>> GetAllMembershipsAsync(CancellationToken ct = default);
        Task<Result> CreateMembershipAsync(CreateMembershipViewModel model , CancellationToken ct = default);
        Task<IEnumerable<PlanSelectViewModel>> GetPlanForDropDown(CancellationToken ct = default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDown(CancellationToken ct = default);
        Task<Result> DeleteActiveMembership(int memberId , CancellationToken ct = default);
        
    }
}
