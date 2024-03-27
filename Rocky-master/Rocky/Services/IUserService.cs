using Rocky_Models.Models;
using System.Security.Claims;

namespace Rocky.Services
{
    public interface IUserService
    {
        public Task RegisterAsync(ApplicationUser user);
        public Task LoginAsync(ApplicationUser user);
        public Task LogoutAsync();
        public bool IsUserSignedIn();
        public int GetUserId();
        public string GetUserName();

        public bool IsInRole(string roleName);
    }
}
