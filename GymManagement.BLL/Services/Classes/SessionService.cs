using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return false;
            if (model.StartDate <= DateTime.Now) return false;
            if (model.Capacity < 1 || model.Capacity > 25) return false;

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null) return false;

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category == null) return false;

            var isValid = Enum.TryParse<Specialties>(category.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return false;

            var session = _mapper.Map<Session>(model);
            _unitOfWork .GetRepository<Session>().AddAsync(session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var Sessions = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategory(ct);
            if (Sessions == null || !Sessions.Any()) return null;

            var MappedSession = Sessions.Select(S => new SessionViewModel()
            {
                Id = S.Id,
                Capacity = S.Capacity,
                CategoryName = S.Category.CategoryName,
                TrainerName = S.Trainer.Name,
                StartDate = S.StartDate,
                EndDate = S.EndDate,
                Description = S.Description
            });

            foreach(var Session in MappedSession)
            {
                Session.AvailableSlots = Session.Capacity - await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(Session.Id, ct);
            }

            return MappedSession;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDown(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }
    }
}
