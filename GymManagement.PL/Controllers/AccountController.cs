using GymManagement.BLL.VeiwModels.AccountViewModels;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly ILogger _logger;

        public AccountController(UserManager<ApplicationUser> userManager , 
                                 SignInManager<ApplicationUser> signInManager )


        {
            _userManager = userManager;
            _signInManager = signInManager;
           // _logger = Logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model , CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            
            if (result.Succeeded)
            {
                //_logger.LogInformation($"User {user.UserName} logged in");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if(result.IsLockedOut)
            {
                //_logger.LogWarning($"User {user.UserName} Logged Out");
                ModelState.AddModelError("InvalidLogin", "This Account Locked out , Try Again Later");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }


        }

    }
}
