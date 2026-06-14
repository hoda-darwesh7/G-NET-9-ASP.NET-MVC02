using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL;
using GymManagement.DAL.Context;
using GymManagement.DAL.Repositories;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
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

        #region Get Plans 
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
        #endregion

        #region Update 

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int PlanId , CancellationToken ct)
        {
            var Plan = await _planService.GetPlanToUpdateAsync(PlanId, ct);
            if (Plan == null)
            {
                TempData["ErrorMessage"] = "Plan Not Found !";
                //return RedirectToAction(nameof(Index));
            }
            return View(Plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdatePlanViewModel model ,  CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);

            var result = await _planService.UpdatePlanAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Plan Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Plan !";

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
