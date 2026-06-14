using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagementBLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<TrainerToUpdateViewModel> GetTrainerToUpdate(int trainerid, CancellationToken ct = default);
        Task<bool> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerid, CancellationToken ct = default);
        Task<bool> DeleteTrainerAsync(int id , CancellationToken ct = default);
    }
}
