using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Services
{
    public class ProductRecommendationService : IProductRecommendationService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUserInteractionRepository _userInteractionRepository;

        public ProductRecommendationService(
            IProductRepository productRepository,
            ILikeRepository likeRepository,
            IOrderDetailRepository orderDetailRepository,
            IUserInteractionRepository userInteractionRepository)
        {
            _productRepository = productRepository;
            _likeRepository = likeRepository;
            _orderDetailRepository = orderDetailRepository;
            _userInteractionRepository = userInteractionRepository;
        }

        public List<Product> GetRecommendedProducts(int userId)
        {
            // Fetch user interactions
            var userInteractions = _userInteractionRepository.GetAll(ui => ui.UserId == userId).ToList();

            // Example logic: Recommend products based on user's likes
            var likedProductIds = userInteractions.Where(ui => ui.InteractionType == "like").Select(ui => ui.ProductId).ToList();

            // Example: recommend similar products (this logic should be more sophisticated in real scenarios)
            var recommendedProducts = _productRepository.GetAll()
                .Where(p => likedProductIds.Contains(p.Id))
                .OrderByDescending(p => likedProductIds.Contains(p.Id))
                .Take(10)
                .ToList();

            return recommendedProducts;
        }
    }
}
