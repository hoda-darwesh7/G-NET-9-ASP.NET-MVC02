using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MembershipsViewModels;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        #region Get 

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var memberShips = await _membershipService.GetAllMembershipsAsync(ct);
            return View(memberShips);
        }


        #endregion

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var member = await _membershipService.GetAllMembershipsAsync(ct);
            await DropDownList(ct);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await DropDownList(ct);
                return View(model);
            }

            var result = await _membershipService.CreateMembershipAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;

            }
            await DropDownList(ct);
            return View(model);
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Cancel(int id , CancellationToken ct)
        {
            var result = await _membershipService.DeleteActiveMembership(id , ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership Canceled Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Helper Method
        private async Task DropDownList(CancellationToken ct)
        {
            var members = await _membershipService.GetMemberForDropDown(ct);
            var plans = await _membershipService.GetPlanForDropDown(ct);

            ViewBag.Members = new SelectList(members , "Id" , "Name");
            ViewBag.Plans = new SelectList(plans , "Id" , "Name");

        }
        #endregion
    }
}
