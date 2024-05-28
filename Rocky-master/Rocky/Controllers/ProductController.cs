
using Microsoft.AspNetCore.Mvc;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rocky.Infrastructure;
using Rocky.Services;
using Microsoft.Extensions.Hosting;


namespace Rocky.Controllers
{
    [OwnAuthorize(WC.AdminRole)]
    public class ProductController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILikeRepository _likeRepository;
        private readonly IUserService _userService;
        private readonly IUserInteractionService _userInteractionService;

        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment, 
            ILikeRepository likeRepository, IUserService userService, IUserInteractionService userInteractionService)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
            _likeRepository = likeRepository;
            _userService = userService;
            _userInteractionService = userInteractionService;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll(includeProperties: "Category")
                .Select(product => new Product
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
                }).ToList();

            return View(products);

        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _productRepository.GetAllDropdownList(WC.CategoryName)
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _productRepository.Find(id.GetValueOrDefault());
                if (productVM == null)
                    return NotFound();

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            //if (ModelState.IsValid)
            //{
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    var objFromDb = _productRepository.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking: false);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _productRepository.Update(productVM.Product);
                }
                TempData[WC.Success] = "Action completed successfully";

                _productRepository.Save();
                return RedirectToAction("Index");
            //}


            //productVM.CategorySelectList = _productRepository.GetAllDropdownList(WC.CategoryName);

            //return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var product = _productRepository.FirstOrDefault(u => u.Id == id, includeProperties: "Category");

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _productRepository.Find(id.GetValueOrDefault());

            if (product == null)
                NotFound();

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);
            

            _productRepository.Remove(product);
            _productRepository.Save();

            TempData[WC.Success] = "Action completed successfully";
            return RedirectToAction("Index");
        }

        // new controllers

        public IActionResult Details(int id)
        {
            var product = _productRepository.FirstOrDefault(p => p.Id == id, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }

            // Log the interaction
            var interaction = new UserInteraction
            {
                UserId = User.GetUserId(), // Assuming you have a method to get the current user's ID
                ProductId = product.Id,
                InteractionType = "view"
            };
            _userInteractionService.LogInteraction(interaction);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Like(int Id)
        {
            var userId = _userService.GetUserId();
            var existing = _likeRepository.FirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == Id);

            if (existing != null)
            {
                _likeRepository.Remove(existing);
            }
            else
            {
                var like = new Like()
                {
                    ApplicationUserId = userId,
                    ProductId = Id
                };

                _likeRepository.Add(like);
            }

            _likeRepository.Save();

            System.Diagnostics.Debug.WriteLine($"User {userId} liked product {Id}");


            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
