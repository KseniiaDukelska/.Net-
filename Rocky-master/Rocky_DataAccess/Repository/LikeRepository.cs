using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;

namespace Rocky_DataAccess.Repository
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private readonly ApplicationDbContext _db;

        public LikeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Like like)
        {
            _db.Likes.Update(like);
        }
    }
}
