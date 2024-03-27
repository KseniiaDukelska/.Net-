using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models.Models;

namespace Rocky_Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderHeaderList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string Status { get; set; }
    }
}
