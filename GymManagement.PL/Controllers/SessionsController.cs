using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(Sessions);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await DropDownList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model ,CancellationToken ct )
        {
            if (!ModelState.IsValid)
            {
                await DropDownList();
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] =result.error;

            }
            await DropDownList();
            return View(model);
        }

        private async Task DropDownList()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDown(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoryForDropDown(), "Id", "CategoryName");
        }

        #endregion
    }
}
