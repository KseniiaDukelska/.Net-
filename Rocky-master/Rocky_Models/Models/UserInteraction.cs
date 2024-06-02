using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky_Models.Models
{
    public class UserInteraction
    {
        [Key]
        public int InteractionId { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [StringLength(50)]
        public string InteractionType { get; set; }

        public float? InteractionValue { get; set; }

        public DateTime InteractionTime { get; set; }
    }
}

