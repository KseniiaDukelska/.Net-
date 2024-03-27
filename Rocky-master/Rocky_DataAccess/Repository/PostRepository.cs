using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;

namespace Rocky_DataAccess.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly ApplicationDbContext _db;

        public PostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Post post)
        {
            _db.Posts.Update(post);
        }
    }
}
