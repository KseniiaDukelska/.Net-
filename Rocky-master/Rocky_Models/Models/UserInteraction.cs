using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Models.Models
{
    public class UserInteraction
    {
        [Key]
        public int InteractionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string InteractionType { get; set; }

        public float? InteractionValue { get; set; } // Optional, e.g., for ratings

        [Required]
        public DateTime InteractionTime { get; set; } = DateTime.Now;
    }
}
