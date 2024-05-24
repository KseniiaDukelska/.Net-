using Rocky_Models.Models;

namespace Rocky.Services
{
    public interface IUserPreferenceService
    {
        void SavePreferences(int userId, List<int> categoryIds);
        bool HasPreferences(int userId);  // Add this method
        List<Category> GetUserPreferences(int userId);  // Add this method if not already present
    }
}
