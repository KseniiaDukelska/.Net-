using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;

namespace Rocky_DataAccess.Repository
{
    public class UserInteractionRepository : Repository<UserInteraction>, IUserInteractionRepository
    {
        private readonly ApplicationDbContext _db;

        public UserInteractionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}