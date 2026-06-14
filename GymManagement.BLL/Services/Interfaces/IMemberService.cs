using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default );

        Task<bool> CreateMemberAsync( CreateMemberViewModel member , CancellationToken ct = default );
        Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default );
        Task<HealthRecordViewModel> GetHealthRecordDetailsAsync(int id , CancellationToken ct = default );
        Task<MemberToUpdateViewModel> GetMemberToUpdateasync( int memberId, CancellationToken ct = default );
        Task<bool> UpdateMemberAsync(int Id , MemberToUpdateViewModel model , CancellationToken ct = default );
        Task<bool> DeleteMemberAsync(int Id , CancellationToken ct = default );
        
    }
}
