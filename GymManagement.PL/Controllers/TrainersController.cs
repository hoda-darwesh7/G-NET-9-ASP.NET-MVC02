using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
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
    }
}
