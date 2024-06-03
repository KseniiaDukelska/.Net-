using Microsoft.AspNetCore.Mvc;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Rocky.Services;
using Rocky_DataAccess.Repository;
using System.Linq;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly ILikeRepository _likeRepository;
        private readonly MLModelPredictionService _predictionService;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserService userService,
            IUserPreferenceService userPreferenceService, ILikeRepository likeRepository, MLModelPredictionService predictionService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userService = userService;
            _userPreferenceService = userPreferenceService;
            _likeRepository = likeRepository;
            _predictionService = predictionService;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll(includeProperties: "Category").ToList();
            var reorderedProducts = products;
            var categories = _categoryRepository.GetAll().OrderBy(c => c.DisplayOrder).ToList();
            var reorderedCategories = categories;

            if (_userService.IsUserSignedIn())
            {
                int userId = _userService.GetUserId(); // Use the extension method to get the current user's ID
                var preferredCategoryIds = _userPreferenceService.GetUserPreferences(userId);

                var scoredProducts = products.Select(product => new
                {
                    Product = product,
                    Score = _predictionService.Predict(new MLModelInput
                    {
                        UserId = userId,
                        ProductId = product.Id // Assuming Product entity has Id property
                    }).Score
                })
                .OrderByDescending(r => r.Score)
                .ToList();

                foreach (var scoredProduct in scoredProducts)
                {
                    _logger.LogInformation($"Product: {scoredProduct.Product.Name}, Score: {scoredProduct.Score}");
                }

                reorderedProducts = scoredProducts.Select(r => r.Product).ToList();
            }

            HomeVM homeVm = new HomeVM()
            {
                Products = reorderedProducts.Select(product => new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ShortDesc = product.ShortDesc,
                    Price = product.Price,
                    Image = product.Image,
                    Category = product.Category,
                    TempSqFt = product.TempSqFt,
                    EndTime = product.EndTime,
                    StartTime = product.StartTime,
                    Place = product.Place,
                    Count = _likeRepository.GetAll(x => x.ProductId == product.Id).Count(),
                }).ToList(),
                Categories = _categoryRepository.GetAll()
            };

            return View(homeVm);
        }

        public IActionResult Details(int Id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _productRepository.FirstOrDefault(u => u.Id == Id, includeProperties: "Category"),
                ExistsInCart = false
            };

            foreach (var item in shoppingCartsList)
            {
                if (item.ProductId == Id)
                {
                    detailsVM.ExistsInCart = true;
                }
            }

            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int Id, DetailsVM details)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartsList.Add(new ShoppingCart { ProductId = Id, SqFt = details.Product.TempSqFt });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);

            TempData[WC.Success] = "Item add to cart successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int Id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppingCartsList.SingleOrDefault(i => i.ProductId == Id);

            if (itemToRemove != null)
            {
                shoppingCartsList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);

            TempData[WC.Success] = "Item removed from cart successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}