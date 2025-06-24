namespace WebAppDemo.Models.ReportModels
{
    public class ReportFilterModel
    {
        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? CategoryId { get; set; }
    }
}
