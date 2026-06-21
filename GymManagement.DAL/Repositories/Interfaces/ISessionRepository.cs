using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetSessionsWithTrainerAndCategory(CancellationToken ct =default);
        Task<int> CountOfBookedSlotsAsync(int id , CancellationToken ct =default);
    }
}
