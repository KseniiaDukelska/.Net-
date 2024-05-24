using Rocky_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky_DataAccess.Repository
{
    public class UserPreferenceRepository : Repository<UserPreference>, IUserPreferenceRepository
    {
        private readonly ApplicationDbContext _db;

        public UserPreferenceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}