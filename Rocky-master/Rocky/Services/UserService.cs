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
                throw new Exception("User with this email or user name already exist");
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
            });
        }

        public bool IsUserSignedIn()
        {
            return _accessor.HttpContext.Request.Cookies.ContainsKey(WC.UserCookie);
        }

        public string GetUserName()
        {
            var claims = GetUserClaimsFromCookie();

            var userName = claims.First(c => c.Key == ClaimsIdentity.DefaultNameClaimType).Value;

            return userName;
        }

        public bool IsInRole(string roleName)
        {
            try
            {
                var claims = GetUserClaimsFromCookie();

                var isInRole = claims.Any(c => c.Key == ClaimsIdentity.DefaultRoleClaimType && c.Value == roleName);

                return isInRole;
            }
            catch (UnauthorizedAccessException e)
            {
                return false;
            }
        }

        public int GetUserId()
        {
            var claims = GetUserClaimsFromCookie();

            var userId = Int32.Parse(claims.First(c => c.Key == ClaimTypes.NameIdentifier).Value);

            return userId;
        }

        private async Task Authenticate(ApplicationUser user)
        {
            var claims = new Dictionary<string, string>()
            {
                { ClaimsIdentity.DefaultNameClaimType, user.UserName },
                { ClaimsIdentity.DefaultRoleClaimType, user.Role.RoleName },
                { ClaimTypes.NameIdentifier, user.Id.ToString() }
            };

            var jsonClaims = JsonConvert.SerializeObject(claims);

            _accessor.HttpContext.Response.Cookies.Append(WC.UserCookie, jsonClaims, new CookieOptions
            {
                Expires = (DateTimeOffset.Now.AddDays(2))
            });
        }

        private Dictionary<string, string> GetUserClaimsFromCookie()
        {
            _accessor.HttpContext.Request.Cookies.TryGetValue(WC.UserCookie, out string jsonClaims);

            if (string.IsNullOrEmpty(jsonClaims))
                throw new UnauthorizedAccessException("User unauthorized");

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonClaims);
        }
    }
}
