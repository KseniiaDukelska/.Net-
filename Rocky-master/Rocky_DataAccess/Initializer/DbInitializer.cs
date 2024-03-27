//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Rocky_Models.Models;
//using Rocky_Utility;

//namespace Rocky_DataAccess.Initializer
//{
//    public class DbInitializer : IDbInitializer
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly ILogger<DbInitializer> _logger;

//        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, 
//                             RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
//        {
//            _db = db;
//            _logger = logger;
//            _roleManager = roleManager;
//            _userManager = userManager;
//        }

//        public void Initialize()
//        {
//            try
//            {
//                if(_db.Database.GetPendingMigrations().Count() > 0)
//                {
//                    _db.Database.Migrate();
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogWarning(ex.Message);
//            }

//            if (!_roleManager.RoleExistsAsync(WC.AdminRole).GetAwaiter().GetResult())
//            {
//                 _roleManager.CreateAsync(new IdentityRole(WC.AdminRole)).GetAwaiter().GetResult();
//                 _roleManager.CreateAsync(new IdentityRole(WC.CastomerRole)).GetAwaiter().GetResult();
//            }
//            else
//            {
//                return;
//            }

//            _userManager.CreateAsync(new ApplicationUser
//            {
//                UserName = "admin@gmail.com",
//                Email = "admin@gmail.com",
//                EmailConfirmed = false,
//                FullName = "Admin Tester",
//                PhoneNumber = "1234567890",
//            }, "Admin123*").GetAwaiter().GetResult();

//            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
//            _userManager.AddToRoleAsync(user, WC.AdminRole).GetAwaiter().GetResult();
            
//        }
//    }
//}
