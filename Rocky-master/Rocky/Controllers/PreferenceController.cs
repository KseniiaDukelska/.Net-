using Microsoft.AspNetCore.Mvc;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Rocky.Services;

namespace Rocky.Controllers
{
    public class PreferenceController : Controller
    {
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;

        public PreferenceController(IUserPreferenceService userPreferenceService, ICategoryRepository categoryRepository, IUserService userService)
        {
            _userPreferenceService = userPreferenceService;
            _categoryRepository = categoryRepository;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Quiz()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePreferences(List<int> categoryIds)
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = _userService.GetUserId();
                System.Diagnostics.Debug.WriteLine($"Saving preferences for UserId: {userId} with Categories: {string.Join(", ", categoryIds)}");
                _userPreferenceService.SavePreferences(userId, categoryIds);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User is not authenticated");
            }

            return RedirectToAction("Confirmation", "Preference");
        }

        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
