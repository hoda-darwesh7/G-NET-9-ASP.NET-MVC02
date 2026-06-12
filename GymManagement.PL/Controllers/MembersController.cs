using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.VeiwModels.MemberViewModel;
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

        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
           => View();

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model , CancellationToken ct )
        {
            if(!ModelState.IsValid) return View(nameof(Create) , model);

            var Result = await _memberService.CreateMemberAsync(model, ct);

            if (Result)
                TempData["SuccessMessage"] = "Member is Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member!";

                return RedirectToAction(nameof(Index));
        }



        #endregion

        #region Edite



        #endregion

        #region Delete



        #endregion



    }
}
