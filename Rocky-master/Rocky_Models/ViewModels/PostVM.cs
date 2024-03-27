using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models.Models;

namespace Rocky_Models.ViewModels
{
    public class PostVM
    {
        public Post Post { get; set; }

        public IEnumerable<SelectListItem> Type = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "Author's article", Text = "Author's article"},
            new SelectListItem(){ Value = "Instruction", Text = "Instruction"},
            new SelectListItem(){ Value = "Technical documentation", Text = "Technical documentation"}
        };
    }
}
