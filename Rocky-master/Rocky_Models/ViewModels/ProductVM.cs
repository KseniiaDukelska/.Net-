using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models.Models;

namespace Rocky_Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
