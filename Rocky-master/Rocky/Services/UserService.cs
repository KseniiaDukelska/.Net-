using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;
using Rocky_Utility;

namespace Rocky.Services
{
    public class UserService : IUserService
    {
        private IHttpContextAccessor _accessor;
        private IApplicationUserRepository _repository;

        public UserService(IHttpContextAccessor accessor, IApplicationUserRepository repository)
        {
            _accessor = accessor;
            _repository = repository;
        }

        public async Task RegisterAsync(ApplicationUser user)
        {
            var isUserExist = _repository.FirstOrDefault(u => u.Email == user.Email || u.UserName == user.UserName);

            if (isUserExist != null)
            {
                throw new Exception("User with this email or user name already exists");
            }

            _repository.Add(user);
            _repository.Save();

            await Authenticate(user);
        }

        public async Task LoginAsync(ApplicationUser user)
        {
            await Authenticate(user);
        }

        public Task LogoutAsync()
        {
            return Task.Run(() =>
            {
                _accessor.HttpContext.Response.Cookies.Delete(WC.UserCookie);
                _accessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            });
        }

        public bool IsUserSignedIn()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public string GetUserName()
        {
            return _accessor.HttpContext.User.Identity.Name;
        }

        public bool IsInRole(string roleName)
        {
            return _accessor.HttpContext.User.IsInRole(roleName);
        }

        public int GetUserId()
        {
            var userIdClaim = _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID claim not found.");
            }

            return int.Parse(userIdClaim);
        }

        private async Task Authenticate(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await _accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}
