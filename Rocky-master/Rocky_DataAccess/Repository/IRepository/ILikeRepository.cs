using Rocky_Models.Models;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface ILikeRepository : IRepository<Like>
    {
        void Update(Like like);
    }
}
