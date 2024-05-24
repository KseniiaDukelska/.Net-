using Microsoft.AspNetCore.Mvc;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Rocky.Services;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;
        private readonly IUserPreferenceService _userPreferenceService;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserService userService,
            IUserPreferenceService userPreferenceService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userService = userService;
            _userPreferenceService = userPreferenceService;
        }

        public IActionResult Index()
        {
            HomeVM homeVm = new HomeVM()
            {
                Products = _productRepository.GetAll(includeProperties: "Category"),
                Categories = _categoryRepository.GetAll()
            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.GetUserId(); // Use the extension method to get the current user's ID
                homeVm.UserPreferences = _userPreferenceService.GetUserPreferences(userId);
            }

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