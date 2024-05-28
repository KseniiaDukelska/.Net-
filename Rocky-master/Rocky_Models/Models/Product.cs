using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky_Models.Models
{
    public class Product
    {
        public Product()
        {
            TempSqFt = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDesc { get; set; }

        [Range(1, int.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }

        [NotMapped]
        [Range(1,10000, ErrorMessage = "Sqft must be greater than 0")]
        public int TempSqFt { get; set; }

        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        // New properties
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Place")]
        public string Place { get; set; }

        public List<Like> Likes { get; set; }

        [NotMapped]
        public int Count { get; set; }
    }
}
