//namespace WebAppDemo.Models.ExcelImportModels
//{
//    public class UserImportResult
//    {
//        public bool IsSuccess { get; set; } = true;

//        public string? Message { get; set; }

//        public int RowNumber { get; set; } //hata hangi satırda 

//        public SignupModel? User { get; set; }
//    }
//}



namespace WebAppDemo.Models.ExcelImportModels
{
    public class UserImportResult
    {
        public bool IsSuccess { get; set; } = true;

        public List<string> Messages { get; set; } = new List<string>();

        public int RowNumber { get; set; } //hata hangi satırda 

        public SignupModel? User { get; set; }
    }
}
