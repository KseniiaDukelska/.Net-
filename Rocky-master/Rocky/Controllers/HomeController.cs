using Microsoft.AspNetCore.Mvc;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Rocky.Services;
using Rocky_DataAccess.Repository;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

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
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly string openAiApiKey = "sk-proj-YDFMosxMy0e6jzNR8MlqT3BlbkFJKlb5IOR9irkOW5Zbbkmr";

        public HomeController(ILogger<HomeController> logger, 
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IUserService userService,
            IUserPreferenceService userPreferenceService,
            ILikeRepository likeRepository, 
            MLModelPredictionService predictionService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userService = userService;
            _userPreferenceService = userPreferenceService;
            _likeRepository = likeRepository;
            _predictionService = predictionService;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll(includeProperties: "Category").ToList();
            var reorderedProducts = products;
            var categories = _categoryRepository.GetAll().ToList();

            if (_userService.IsUserSignedIn())
            {
                int userId = _userService.GetUserId(); // Use the extension method to get the current user's ID
                var preferredCategoryIds = _userPreferenceService.GetPreferences(userId);

                int order = 1;
                foreach (var category in categories)
                {
                    if (preferredCategoryIds.Contains(category.Id))
                    {
                        category.DisplayOrder = order++;
                    }
                    else
                    {
                        category.DisplayOrder = order + 100; // Push non-preferred categories down the order
                    }
                }

                // Sort categories by the updated DisplayOrder values
                categories = categories.OrderBy(c => c.DisplayOrder).ToList();

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
            else
            {
                categories = categories.OrderBy(c => c.DisplayOrder).ToList(); // Ensure categories are ordered even when not logged in
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
                    AgeRestriction = product.AgeRestriction,
                    Count = _likeRepository.GetAll(x => x.ProductId == product.Id).Count(),
                }).ToList(),

                Categories = categories
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

        [HttpPost]
        public async Task<IActionResult> SearchEvents(string query)
        {
            string openAiResponse = await GetOpenAiResponse(query);
            _logger.LogInformation($"OpenAI Response: {openAiResponse}");
            var keywords = ExtractKeywordsFromOpenAiResponse(openAiResponse);
            var events = FilterEventsByKeywords(keywords);
            return PartialView("_SearchResults", events);
        }

        private async Task<string> GetOpenAiResponse(string query)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");

            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are an assistant that helps users find suitable events. Consider all event details such as name, description, start time, end time, place, category, price, and age restrictions. The query and event details are in Ukrainian. Return a list of keywords for filtering events based on the user's query." },
                    new { role = "user", content = query }
}
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JObject.Parse(responseString);

            return responseObject.choices[0].message.content;
        }


        private List<Product> FilterEventsByKeywords(List<string> keywords)
        {
            var allEvents = _productRepository.GetAll(includeProperties: "Category").ToList();
            bool isAdultQuery = keywords.Contains("adult") || keywords.Contains("дорослих");
            var events = allEvents
                .Where(p =>
                  (isAdultQuery ? p.AgeRestriction >= 18 : (!p.AgeRestriction.HasValue || p.AgeRestriction < 18)) && // Check age restriction
                    keywords.Any(k =>
                        p.Description.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                        p.Category.Name.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                        p.Place.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                        p.StartTime.ToString().Contains(k, StringComparison.OrdinalIgnoreCase) ||
                        (p.EndTime.HasValue && p.EndTime.Value.ToString().Contains(k, StringComparison.OrdinalIgnoreCase))
                    )
                )
                .ToList();

            return events;
        }

        private List<string> ExtractKeywordsFromOpenAiResponse(string response)
        {
            // Assuming the response is a comma-separated list of keywords
            var keywords = response.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(k => k.Trim())
                                   .ToList();
            return keywords;
        }


    }
}