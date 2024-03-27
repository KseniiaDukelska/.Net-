using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;


namespace Rocky_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

       public void Update(Category category)
       {
            var objFromDb = base.FirstOrDefault(u => u.Id == category.Id);

            if (objFromDb != null) 
            {
                objFromDb.Name = category.Name;
                objFromDb.DisplayOrder = category.DisplayOrder;
            }
       }
    }
}
