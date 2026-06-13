using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{

    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        #region Get Members

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllAsync(ct);
            return View(members);
        }

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
            }
            return View(member);
        }

        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var memberrecord = await _memberService.GetHealthRecordDetailsAsync(id, ct);
            if (memberrecord == null)
            {
                TempData["ErrorMessage"] = "Member Health Record Not Found !";
                return RedirectToAction(nameof(Index));
            }
            return View(memberrecord);
        }
        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
           => View();

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var Result = await _memberService.CreateMemberAsync(model, ct);

            if (Result)
                TempData["SuccessMessage"] = "Member is Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member!";

            return RedirectToAction(nameof(Index));
        }



        #endregion

        #region Edite

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateasync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Index !";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete



        #endregion



    }
}
