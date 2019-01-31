using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VERSUS.Infrastructure.Models;

namespace VERSUS.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SiteUser> _userManager;

        public AccountController(UserManager<SiteUser> userManager)
        {
            _userManager = userManager;
        }

        [Route("/login")]
        public async Task<IActionResult> LogIn([FromBody]UserCrendentials siteUser)
        {
            var newUser = new SiteUser
            {
                UserName = siteUser.UserName
            };

            var userCreationResult = await _userManager.CreateAsync(newUser, siteUser.UserPassword);
            var userCredentialsResult = UserCredentialsResult.Success;

            if (!userCreationResult.Succeeded)
            {
                userCredentialsResult = UserCredentialsResult.Error;
            }

            return Json(
                new
                {
                    result = userCredentialsResult,
                    errors = userCreationResult.Errors
                }
            );
        }
    }
}