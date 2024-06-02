using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Infrastructure;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;
using Rocky_Utility;

namespace Rocky.Controllers
{
    [OwnAuthorize(WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _categoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(category);
                _categoryRepository.Save();

                TempData[WC.Success] = "Category create successfully";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Error while creating category";

            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = _categoryRepository.Find(id.GetValueOrDefault());

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                _categoryRepository.Save();

                TempData[WC.Success] = "Action completed successfully";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = _categoryRepository.Find(id.GetValueOrDefault());

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _categoryRepository.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                NotFound();
            }

            TempData[WC.Success] = "Action completed successfully";

            _categoryRepository.Remove(obj);
            _categoryRepository.Save();

            return RedirectToAction("Index");
        }


    }
}
