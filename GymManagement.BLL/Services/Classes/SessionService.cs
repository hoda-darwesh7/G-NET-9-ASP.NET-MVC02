using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.SessionViewModels;
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

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
