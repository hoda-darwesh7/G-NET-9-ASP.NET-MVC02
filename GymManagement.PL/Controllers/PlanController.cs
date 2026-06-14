using GymManagement.DAL;
using GymManagement.DAL.Context;
using GymManagement.DAL.Repositories;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepository;

            public PlanController(IGenericRepository<Plan> planRepository)
            {
                _planRepository = planRepository;
            }

        // GET ::BaseUrl/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepository.GetAllAsync(ct:ct);
            return View(plans);
        }

        // GET ::BaseUrl/Plan/Details
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id,ct);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

    }
}
