namespace WebAppDemo.Models.ReportModels
{
    public class ReportViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public DateTime OpenDate { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; } = string.Empty;


    }
}
