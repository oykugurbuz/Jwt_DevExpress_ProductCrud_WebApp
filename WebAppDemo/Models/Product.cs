using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage="Ürün adını boş bırakamazsınız!!")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Ürün fiyatını boş bırakamazsınız!!")]
        [Range(1,1000000,ErrorMessage = "Ürün fiyatı 1 ile 1000000 arasında olmalıdır!!")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Price { get; set; }
        public string Description { get; set; } =string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime OpenDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli bir kategori seçin.")]
        public int  CategoryId { get; set; } 
        public Category Category { get; set; }
    }
}
