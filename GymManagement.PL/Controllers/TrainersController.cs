using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagementBLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }


        #region Get Trainers
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Trainers = await _trainerService.GetAllTrainersAsync(ct);
            return View(Trainers);
        }

        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create()
          => View();

        [HttpPost]
        public async Task<IActionResult> CreatTrainer(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _trainerService.CreateTrainerAsync(model, ct);

            if (result)
                TempData["SuccessMessage"] = "Trainer is Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Create Trainer!";

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
