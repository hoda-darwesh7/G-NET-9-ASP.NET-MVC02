using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Context;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanRepository PlanRepository;


        // GET ::BaseUrl/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await PlanRepository.GetAllAsync(ct:ct);
            return View(plans);
        }

        // GET ::BaseUrl/Plan/Details
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var plan = await PlanRepository.GetByIdAsync(id,ct);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

    }
}
