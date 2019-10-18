using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Name is too long.")]
        public string Label { get; set; }
        [Required]
        [StringLength(4000, ErrorMessage = "Details is too long.")]
        public string Details { get; set; }
        [Required]
        public byte[] Picture { get; set; }
        [Required]
        [Range(0, 999999999, ErrorMessage = "Accommodation invalid (0-999999999)")]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; }
        public int? Duration { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
