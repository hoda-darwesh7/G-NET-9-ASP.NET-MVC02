using GymManagement.BLL.Services.Interfaces;
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
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        // GET ::BaseUrl/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);
            return View(plans);
        }

        // GET ::BaseUrl/Plan/Details
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var plan = await _planService.GetPlanDetailsByIdAsync(id,ct);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

    }
}
