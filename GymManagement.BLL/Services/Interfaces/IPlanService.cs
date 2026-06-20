using GymManagement.BLL.VeiwModels.PlanViewModel;
using GymManagementBLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsByIdAsync(int PlanId , CancellationToken ct = default); 
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int PlanId , CancellationToken ct = default);
        Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel Model , CancellationToken ct = default);
        Task<bool> ToggleActivationAsync(int id , CancellationToken ct = default);
    }
}
