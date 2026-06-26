using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagementBLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
            }
            return View(trainer);
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

        #region Update

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int  id , CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerToUpdate(id, ct);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel Model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(Model);

            var result = await _trainerService.UpdateTrainerAsync(id, Model, ct);
            if (result)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Index !";
            return RedirectToAction(nameof(Index));
        }


        #endregion

        #region Delete 

        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);

            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var Result = await _trainerService.DeleteTrainerAsync(id, ct);
            if (Result)
                TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Trainer !";

            return RedirectToAction(nameof(Index));

        }

        #endregion
    }
}
