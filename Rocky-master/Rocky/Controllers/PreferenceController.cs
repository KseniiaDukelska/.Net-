using Microsoft.AspNetCore.Mvc;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocky.Infrastructure;
using Rocky.Services;

namespace Rocky.Controllers
{
    public class PreferenceController : Controller
    {
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly ICategoryRepository _categoryRepository;

        public PreferenceController(IUserPreferenceService userPreferenceService, ICategoryRepository categoryRepository)
        {
            _userPreferenceService = userPreferenceService;
            _categoryRepository = categoryRepository;
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
            var userId = User.GetUserId(); // Use the extension method to get the current user's ID
            System.Diagnostics.Debug.WriteLine($"Saving preferences for UserId: {userId}");
            _userPreferenceService.SavePreferences(userId, categoryIds);
            return RedirectToAction("Confirmation", "Preference");
        }


        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
