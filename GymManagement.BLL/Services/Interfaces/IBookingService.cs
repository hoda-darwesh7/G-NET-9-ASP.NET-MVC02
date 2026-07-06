using GymManagement.BLL.VeiwModels.BookingViewModels;
using GymManagement.BLL.VeiwModels.Common;
using GymManagement.BLL.VeiwModels.MembershipsViewModels;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<MemberForSessionViewModel>>> GetMemberForSessionAsync( int sessionId , CancellationToken ct = default);
        Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMemberForDropDown( int sessionId , CancellationToken ct = default);
        Task<Result> CreateBookingAsync(CreateBookingViewModel model , CancellationToken ct = default);
        Task<Result> IsAttendedAsync(int sessionId , int memberId , CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int sessionId , int memberId , CancellationToken ct = default);
    }
}
