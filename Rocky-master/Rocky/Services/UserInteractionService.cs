using Rocky_DataAccess;
using Rocky_Models.Models;

namespace Rocky.Services
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly ApplicationDbContext _db;

        public UserInteractionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public void LogInteraction(UserInteraction interaction)
        {
            _db.UserInteractions.Add(interaction);
            _db.SaveChanges();
        }
    }
}
