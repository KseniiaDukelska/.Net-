using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky_Models.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        public int ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

 
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]

        public Product Product { get; set; }

    }
}
