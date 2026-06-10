using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Context;

namespace GymManagement.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly GymDbContext  context;

        public PlanController()
        {
            context = new GymDbContext();
        }

        // GET ::BaseUrl/Plan/Index
        public async Task<IActionResult> Index()
        {
            var plans = await context.Plans.ToListAsync();
            return View(plans);
        }

        // GET ::BaseUrl/Plan/Details
        public async Task<IActionResult> Details(int id)
        {
            var plan = await context.Plans.FindAsync(id);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

    }
}
