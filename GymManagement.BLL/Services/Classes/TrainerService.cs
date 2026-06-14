using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (!trainers.Any())
                return [];

            List<TrainerViewModel> result = new List<TrainerViewModel>();

            foreach (var trainer in trainers)
            {
                var trainerViewModel = new TrainerViewModel()
                {
                    Name = trainer.Name,
                    Phone = trainer.Phone,
                    Email = trainer.Email,
                    Id = trainer.Id,
                    Gender = trainer.Gender.ToString(),
                    Specialties = trainer.Specialty.ToString()
                };
                result.Add(trainerViewModel);
            }
            return result;
        }
    }
}
