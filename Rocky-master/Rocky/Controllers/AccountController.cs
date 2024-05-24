using Microsoft.AspNetCore.Mvc;
using Rocky.Infrastructure;
using Rocky.Services;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Syncfusion.EJ2.DropDowns;
using Syncfusion.EJ2.FileManager;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;
        private readonly IUserPreferenceService _userPreferenceService;

        public AccountController(IApplicationUserRepository userRepository, IRoleRepository roleRepository, IUserService userService, IUserPreferenceService userPreferenceService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userService = userService;
            _userPreferenceService = userPreferenceService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegistrationVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName,
                    Password = model.Password
                };

                if (_userService.IsInRole(WC.AdminRole))
                {
                    user.Role = _roleRepository.FirstOrDefault(r => r.RoleName == WC.AdminRole);
                }
                else
                {
                    user.Role = _roleRepository.FirstOrDefault(r => r.RoleName == WC.CastomerRole);
                }

                try
                {
                    await _userService.RegisterAsync(user);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CheckError", ex.Message);
                    return View(model);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVW model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.FirstOrDefault(u => u.Email == model.Email, "Role");

                if (user == null)
                {
                    ModelState.AddModelError("CheckError", "User with this email does not exist");
                    return View(model);
                }

                if (model.Password != user.Password)
                {
                    ModelState.AddModelError("CheckError", "Wrong password!");
                    return View(model);
                }

                await _userService.LoginAsync(user);

                // Check if the user has completed the preference quiz
                int userId = user.Id;  // No need to parse since it's already an int
                if (!_userPreferenceService.HasPreferences(userId))
                {
                    return RedirectToAction("Quiz", "Preference");
                }


                return RedirectToAction("Index", "Home");
            }


            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            if (_userService.IsUserSignedIn())
            {
                await _userService.LogoutAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
