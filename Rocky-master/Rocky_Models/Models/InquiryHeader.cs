﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky_Models.Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get; set; }

        public int ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime InquiryDate { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
