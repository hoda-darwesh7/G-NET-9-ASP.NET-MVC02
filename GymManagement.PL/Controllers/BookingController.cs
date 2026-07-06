using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.BookingViewModels;
using GymManagement.BLL.VeiwModels.MembershipsViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _bookingService.GetAllSessionAsync(ct);

            return View(sessions.value);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersForUpcomingSession(int id , CancellationToken ct)
        {
            var members = await _bookingService.GetMemberForSessionAsync(id, ct);

            return View(members.value);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersForOngoingSessions(int id , CancellationToken ct)
        {
            var members = await _bookingService.GetMemberForSessionAsync(id, ct);

            return View(members.value);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id , CancellationToken ct)
        {
            var members = await GetMemberForDropDowns(id, ct);
            ViewBag.Members = new SelectList(members , "Id" , "Name");

            var model = new CreateBookingViewModel
            {
                SessionId = id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model ,  CancellationToken ct)
        {


            var result = await _bookingService.CreateBookingAsync(model , ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Booking Created Successfuly";
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession) , new {id = model.SessionId});
        }

        [HttpPost]
        public async Task<IActionResult> Attended(int sessionId , int memberId , CancellationToken ct)
        {
            var result = await _bookingService.IsAttendedAsync(sessionId , memberId , ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Member Marked As Attended Successfuly";
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
            }
            return RedirectToAction(nameof(GetMembersForOngoingSessions), new { id = sessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int sessionId, int memberId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(sessionId, memberId, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Booking Canceled Successfuly";
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
            }
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
        }

        private async Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDowns(int id , CancellationToken ct)
        {
            var members = await _bookingService.GetMemberForDropDown(id, ct);
            return members.value; 
        }
    }
}
