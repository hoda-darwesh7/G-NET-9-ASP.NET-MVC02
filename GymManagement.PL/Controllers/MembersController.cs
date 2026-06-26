using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService , IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
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

        // Get Member Photo
        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member is null || string.IsNullOrWhiteSpace(member.Photo))
                return NotFound();
            var result = _attachmentService.GetFile(member.Photo, "MembersPhoto");
            if(result == null) return NotFound();
            return File(result.Value.stream , result.Value.contentType);
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
        public async Task<IActionResult> EditMember([FromRoute] int id, CancellationToken ct)
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

        public async Task<IActionResult> Delete([FromRoute] int id , CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found !";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id , CancellationToken ct)
        {
            var Result = await _memberService.DeleteMemberAsync(id, ct);
            if(Result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Member !";

            return RedirectToAction(nameof(Index));

        }

        #endregion



    }
}
