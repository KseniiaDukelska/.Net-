using Rocky_Models.Models;

namespace Rocky.Services
{
    public interface IUserInteractionService
    {
        void LogInteraction(UserInteraction interaction);
    }
}
