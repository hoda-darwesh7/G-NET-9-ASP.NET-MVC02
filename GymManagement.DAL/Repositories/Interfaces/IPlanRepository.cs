using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        //GetAllPlans
        Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false , CancellationToken ct = default);
        //GetPlanById
        Task<Plan> GetByIdAsync(int id , CancellationToken ct = default);
        //Add
        Task<int> AddAsync(Plan plan , CancellationToken ct = default);
        //Update
        Task<int> UpdateAsync(Plan plan , CancellationToken ct = default);
        //Delete
        Task<int> DeleteAsync(Plan plan, CancellationToken ct = default);
    }
}
