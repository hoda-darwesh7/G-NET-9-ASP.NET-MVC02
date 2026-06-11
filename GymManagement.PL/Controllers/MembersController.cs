using GymManagement.BLL.Services.Interfaces;
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



        #endregion

        #region Edite



        #endregion

        #region Delete



        #endregion



    }
}
