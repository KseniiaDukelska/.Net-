using Rocky_Models.Models;
using System.Collections.Generic;

namespace Rocky.Services
{
    public interface IProductRecommendationService
    {
        List<Product> GetRecommendedProducts(int userId);
    }
}
