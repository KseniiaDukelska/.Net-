using Rocky_DataAccess;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;

namespace Rocky.Services
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly IUserInteractionRepository _userInteractionRepository;

        public UserInteractionService(IUserInteractionRepository userInteractionRepository)
        {
            _userInteractionRepository = userInteractionRepository;
        }

        public void LogInteraction(UserInteraction interaction)
        {
            _userInteractionRepository.Add(interaction);
            _userInteractionRepository.Save();
        }

        public IEnumerable<UserInteraction> GetUserInteractions(int userId)
        {
            return _userInteractionRepository.GetAll(ui => ui.UserId == userId).ToList();
        }
    }
}
