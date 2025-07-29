namespace WebAppDemo.Models.ExcelImportModels
{
    public class UserImportResult
    {
        public bool IsSuccess { get; set; } = true;

        public string? Message { get; set; }

        public int RowNumber { get; set; } //hata hangi satırda 

        public SignupModel? User { get; set; }
    }
}
