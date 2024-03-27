using Rocky_Models.Models;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IPostRepository : IRepository<Post>
    {
        void Update(Post post);
    }
}
