using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Rocky_Utility;

namespace Rocky.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OwnAuthorizeAttribute : TypeFilterAttribute
    {
        public OwnAuthorizeAttribute(string? roleValue = null) : base(typeof(OwnAuthorizationFilter))
        {
            if (roleValue != null)
            {
                Arguments = new object[] { new KeyValuePair<string, string>(ClaimsIdentity.DefaultRoleClaimType, roleValue) };
            }
        }

    }

    public class OwnAuthorizationFilter : IAuthorizationFilter
    {
        private readonly KeyValuePair<string, string>? _claim;

        public OwnAuthorizationFilter(KeyValuePair<string, string>? claim = null)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Cookies.TryGetValue(WC.UserCookie, out string jsonClaims);

            if (string.IsNullOrEmpty(jsonClaims))
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            if (_claim != null)
            {
                var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonClaims);



                var hasRole = claims.Any(c => c.Key == ClaimsIdentity.DefaultRoleClaimType && c.Value == _claim.Value.Value);

                if (!hasRole)
                {
                    context.Result = new ForbidResult();
                }
            }

        }
    }
}
