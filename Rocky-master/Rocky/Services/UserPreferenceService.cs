using Rocky_Models;
using Rocky_DataAccess;
using System.Collections.Generic;
using System.Linq;
using Rocky_Models.Models;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UserPreferenceService(ApplicationDbContext db, IUserPreferenceRepository userPreferenceRepository, ICategoryRepository categoryRepository)
        {
            _db = db;
            _userPreferenceRepository = userPreferenceRepository;
            _categoryRepository = categoryRepository;
        }

        public void SavePreferences(int userId, List<int> categoryIds)
        {
            try
            {
                var existingPreferences = _db.UserPreferences.Where(up => up.UserId == userId);
                _db.UserPreferences.RemoveRange(existingPreferences);

                foreach (var categoryId in categoryIds)
                {
                    _db.UserPreferences.Add(new UserPreference
                    {
                        UserId = userId,
                        CategoryId = categoryId
                    });
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving preferences: {ex.Message}");
            }
        }


        public bool HasPreferences(int userId)
        {
            return _db.UserPreferences.Any(up => up.UserId == userId);
        }

        public List<int> GetPreferences(int userId)
        {
            return _db.UserPreferences
                      .Where(up => up.UserId == userId)
                      .Select(up => up.CategoryId)
                      .ToList();
        }

        public List<Category> GetUserPreferences(int userId)
        {
            var categoryIds = _userPreferenceRepository.GetAll(up => up.UserId == userId).Select(up => up.CategoryId).ToList();
            return _categoryRepository.GetAll(c => categoryIds.Contains(c.Id)).ToList();
        }
    }
}
