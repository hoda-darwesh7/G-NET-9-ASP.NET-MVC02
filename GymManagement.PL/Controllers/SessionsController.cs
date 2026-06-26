using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }


        #region Get

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(Sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var Sessions = await _sessionService.GetSessionByIdAsync(id , ct);
            if (Sessions.success)
                return View(Sessions.value);
            else
            {
                TempData["ErrorMessage"] = Sessions.error;
                return RedirectToAction("Index");
            }
        }

        #endregion


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


        #region Update 

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (result.success)
            {
                await GetTrainerlist();
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await GetTrainerlist();
                return View(model);
            }

            var RResult = await _sessionService.UpdateSessionAsync(id, model, ct);
            if(RResult.success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfuly";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = RResult.error;
                await GetTrainerlist();
                return View(model);
            }
        }

        private async Task GetTrainerlist()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDown() , "Id" ,"Name");
        }

        #endregion
    }
}
