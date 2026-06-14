using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.PlanViewModel;
using GymManagement.DAL;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);
            if (!plans.Any())
                return [];

            List<PlanViewModel> result = new List<PlanViewModel>();

            foreach (var plan in plans)
            {
                var PlanViewModel = new PlanViewModel()
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price,
                    IsActive = plan.IsActive,
                };
                result.Add(PlanViewModel);
            }
            return result;
        }

        public async Task<PlanViewModel?> GetPlanDetailsByIdAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId , ct);
            if (plan == null) return null;

            var Model = new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive,
            };
            return Model;
           
        }

        public async Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (Plan == null) return null;
            else
                return new UpdatePlanViewModel()
                {
                    
                    PlanName = Plan.Name,
                    Price = Plan.Price,
                    Description = Plan.Description,
                    DurationDays = Plan.DurationDays,
                };

        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel Model, CancellationToken ct = default)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);


            if (Plan == null) return false;
            else
            {
                Plan.Name = Model.PlanName;
                Plan.Description = Model.Description;
                Plan.DurationDays = Model.DurationDays;
                Plan.Price = Model.Price;
            }
            _unitOfWork.GetRepository<Plan>().UpdateAsync(Plan);
            var Result = await _unitOfWork.SaveChangesAsync(ct);
            return Result > 0;
        }
    }
}
