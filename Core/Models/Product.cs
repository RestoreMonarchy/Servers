using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class Product
    {
        public short ProductId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string ShortName { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
        [Required]
        [StringLength(2000, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }
        [Required]
        [Range(0, 999999999, ErrorMessage = "Accommodation invalid (0-999999999)")]
        public decimal Price { get; set; }
        public short? RankId { get; set; }
        public decimal Coins { get; set; }
        public string Category { get; set; }
        public DateTime CreateDate { get; set; }
        public bool ActiveFlag { get; set; }

        public virtual Rank Rank { get; set; }
    }
}
