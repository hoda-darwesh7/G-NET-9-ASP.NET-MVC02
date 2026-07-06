using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.AnalyticsViewModels;
using GymManagement.DAL;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upcommingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(x=>x.StartDate > now);
            var ongoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(x=>x.StartDate <= now && x.EndDate >= now);
            var completedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(x=>x.EndDate < now);
            var members = await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct);
            var trainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var activeMembers = await _unitOfWork.GetRepository<MemberShip>().CountAsync(x=>x.EndDate > now , ct);

            return new AnalyticsViewModel()
            {
                TotalMembers = members,
                TotalTrainers = trainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = upcommingSessions,
                CompletedSessions = completedSessions,
                OngoingSessions = ongoingSessions
            };

        }
    }
}
