//using DevExpress.Office.Utils;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System.Text.Json;
//using WebAppDemo.Models;

//namespace WebAppDemo.Controllers
//{
//    [Authorize]
//    public class UserController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public UserController(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
//        {
//            var userName = User.Identity?.Name; 
//            ViewBag.UserName = userName;
//            var currentAuthorityLevel = User.Claims.FirstOrDefault(c => c.Type == "AuthorityLevel")?.Value;
//            ViewBag.AuthorityLevel = currentAuthorityLevel;

//            if (excelFile == null || excelFile.Length == 0) 
//            {
//                return View();
//            }

//            var userList = new List<SignupModel>(); 
//            var errorUsers = new List<SignupModel>(); 
//            using (var stream = new MemoryStream()) 
//            {
//                await excelFile.CopyToAsync(stream); 

//                using (var package = new ExcelPackage(stream)) 
//                {
//                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); 

//                    if(worksheet == null) 
//                    {
//                        ViewBag.UploadError = "Excel sayfası bulunamadı!";
//                        return View();
//                    }
//                    if(worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
//                    {
//                        ViewBag.UploadError = "Excel dosyası boş görünüyor. Lütfen en az bir kullanıcı kaydı içeren bir dosya yükleyin.";
//                        return View();
//                    }
//                    int rowCount = worksheet.Dimension.Rows; 

//                    for(int row= 2; row <= rowCount; row++) 
//                    {
//                        var user = new SignupModel 
//                        {
//                            UserName = worksheet.Cells[row, 1].Text?.Trim(), 
//                            IdentityNumber = long.Parse(worksheet.Cells[row, 2].Text?.Trim() ?? "0"),
//                            Email = worksheet.Cells[row, 3].Text?.Trim(),
//                            Password = worksheet.Cells[row, 4].Text?.Trim(),
//                            AuthorityLevel= int.Parse(worksheet.Cells[row,5].Text?.Trim())
//                        };
//                        userList.Add(user); 
//                    }

//                    using (var httpClient = new HttpClient()) //otomatik olarak disponse edilir.
//                    {
//                        httpClient.BaseAddress = new Uri("http://localhost:5269");
//                        foreach(var user in userList) 
//                        {
//                            var json = JsonSerializer.Serialize(user); 
//                            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"); 

//                            var response = await httpClient.PostAsync("api/Auth/signup/", content); 

//                            if(!response.IsSuccessStatusCode)
//                            {
//                                errorUsers.Add(user);

//                            }
//                        }


//                        }
//                    if (errorUsers.Any())
//                    {
//                        TempData["ErrorUsers"] = JsonSerializer.Serialize(errorUsers);

//                        ViewBag.UploadError = $"Toplam {errorUsers.Count} hatalı kayıt var. Lütfen 'Hatalı Kayıtları İndir' butonuyla dosyayı indirin.";
//                        return View();

//                    }


//                    ViewBag.UploadMessage = "Kullanıcı kaydı başarılı!";
//                    return View();


//                }
//            }
//        }
//        public IActionResult DownloadErrorExcel()
//        {
//            if (!TempData.ContainsKey("ErrorUsers"))
//                return RedirectToAction("UploadExcel");

//            var errorUsersJson = TempData["ErrorUsers"] as string;
//            if (string.IsNullOrEmpty(errorUsersJson))
//                return RedirectToAction("UploadExcel");

//            var errorUsers = JsonSerializer.Deserialize<List<SignupModel>>(errorUsersJson);
//            var errorExcel = CreateErrorExcel(errorUsers);
//            var fileName = $"HatalıKayıtlar_{DateTime.Now: dd.MM.yyyy_HH.mm}.xlsx";

//            return File(errorExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
//        }
//        private byte[] CreateErrorExcel(List<SignupModel> errors)
//        {
//            using (var package = new ExcelPackage())
//            {
//                var worksheet = package.Workbook.Worksheets.Add("Hatalı Kayıtlar"); 
//                worksheet.Cells[1,1].Value =  "Kullanıcı Adı";
//                worksheet.Cells[1, 2].Value = "Kimlik Numarası";
//                worksheet.Cells[1, 3].Value = "Email";
//                worksheet.Cells[1, 4].Value = "Parola";
//                worksheet.Cells[1, 5].Value = "Kullanıcı Seviyesi";
//                int row = 2; 
//                foreach (var error in errors) 
//                {
//                    worksheet.Cells[row, 1].Value = error.UserName; 
//                    worksheet.Cells[row, 2].Value = error.IdentityNumber;
//                    worksheet.Cells[row, 3].Value = error.Email; 
//                    worksheet.Cells[row, 4].Value = error.Password;
//                    worksheet.Cells[row, 5].Value = error.AuthorityLevel;
//                    row++; //bir sonraki satıra geçiyoruz.
//                }
//                return package.GetAsByteArray(); // Excel paketini byte dizisi olarak döndür
//            }
//        }

//        public IActionResult UserList()
//        {


//            var userList = _context.AppUserInfos.Select(u => new UserList
//            {
//                Id = u.Id,
//                UserName = u.UserName,
//                Email = u.Email,
//                AuthorityLevel = u.AuthorityLevel,
//                //IdentityNumber = u.AuthorityLevel == 1 ? u.IdentityNumber : null
//                IdentityNumber = u.IdentityNumber


//            }).ToList();

//            return Json(userList);
//        }


//    }


//}










//2................
//using DevExpress.ClipboardSource.SpreadsheetML;
//using DevExpress.Office.Utils;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System.Text;
//using System.Text.Json;
//using WebAppDemo.Models;
//using WebAppDemo.Models.ExcelImportModels;

//namespace WebAppDemo.Controllers
//{
//    [Authorize]
//    public class UserController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        public UserController(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public IActionResult UploadExcel()
//        {
//            return View();
//        }
//        [HttpPost]
//        public async Task<IActionResult>UploadExcel(IFormFile excelFile)
//        {
//            if (excelFile == null || excelFile.Length == 0)
//                return Json(new { success = false, message = "Excel dosyası boş." });

//            using var stream = new MemoryStream();
//            await excelFile.CopyToAsync(stream);
//            using var package = new ExcelPackage(stream);
//            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

//            if (worksheet?.Dimension == null || worksheet.Dimension.Rows <2)
//                return Json(new { success = false, message = "Excel boş ya da yeterli sayıda kayıt yok." });

//            var headers = new List<string>();
//            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
//                headers.Add(worksheet.Cells[1, col].Text);

//            TempData["ExcelFile"] = Convert.ToBase64String(stream.ToArray());

//            return Json(new { success = true, headers = headers });
//        }

//        public async Task<IActionResult> ColumnMappingResult(IFormCollection form)
//        {
//            //önceki methodda tempdatayı byte gibi karmaşık nesneleri saklayamayacağı için tobase64 yaptık o değer tempexcel 
//            var tempExcel = TempData["ExcelFile"] as string;
//            //veri yoksa sayfa yenilenmiş olabilir vs. tekrar yükleme sayfasına dön
//            if (tempExcel == null)
//                return RedirectToAction("UploadExcel");
//            //base 64 ten tekrar byte dizisine çeviriyoruz
//            byte[] excelBytes = Convert.FromBase64String(tempExcel);
//            //byte dizisinde bulunan excelBytes rame kaydediyoruz
//            using var stream = new MemoryStream(excelBytes);
//            //epplus kütüphanesiyle excel dosyasını açıyoruz
//            using var package = new ExcelPackage(stream);

//            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

//            if (worksheet.Dimension == null)
//            {
//                return Json(new { succees = false, message = "Excel boş ya da yeterli sayıda kayıt yok." });
//            }



//            var mapping = new Dictionary<string, string>();

//            foreach (var key in form.Keys)
//            {
//                if (key.EndsWith("Column"))
//                {
//                    var fieldName = key.Replace("Column", "");
//                    var excelHeader = form[key];

//                    if (!string.IsNullOrEmpty(excelHeader))
//                    {
//                        mapping[fieldName] = excelHeader;
//                    }
//                }
//            }
//            var results = new List<UserImportResult>();

//            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
//            {
//                var user = new SignupModel();
//                var result = new UserImportResult
//                {
//                    RowNumber = row
//                };

//                try
//                {
//                    foreach (var map in mapping)
//                    {
//                        var alan = map.Key;
//                        var excelCol = map.Value;

//                        int colIndex = -1;
//                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
//                        {
//                            if (worksheet.Cells[1, col].Text == excelCol)
//                            {
//                                colIndex = col;
//                                break;
//                            }
//                        }

//                        var cellValue = worksheet.Cells[row, colIndex].Text;

//                        switch (alan)
//                        {
//                            case "TCKimlikNumarası":
//                                user.IdentityNumber = long.TryParse(cellValue, out var tc) ? tc : 0;
//                                break;
//                            case "KullanıcıAdı":
//                                user.UserName = cellValue;
//                                break;
//                            case "E-Mail":
//                                user.Email = cellValue;
//                                break;
//                            case "Şifre":
//                                user.Password = cellValue;
//                                break;
//                            case "KullanıcıSeviyesi":
//                                user.AuthorityLevel = int.TryParse(cellValue, out var level) ? level : null;
//                                break;
//                        }
//                    }

//                    var apiResult = await CallSignupApi(user);
//                    result.User = user;
//                    result.Message = apiResult;
//                }
//                catch (Exception ex)
//                {
//                    result.User = user;
//                    result.IsSuccess = false;
//                    result.Message = "Hata: " + ex.Message;
//                }

//                results.Add(result);
//            }


//            return View("ImportResult", results);
//        }

//        private async Task<string> CallSignupApi(SignupModel user)
//        {
//            using var client = new HttpClient();
//            var json = JsonSerializer.Serialize(user);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");

//            var response = await client.PostAsync("http://localhost:5269/api/Auth/signup", content);

//            if (response.IsSuccessStatusCode)
//            {
//                return "";
//            }
//            else
//            {
//                return  await response.Content.ReadAsStringAsync();
//            }
//        }
//        public IActionResult ImportResult()
//        {
//            var userName = User.Identity?.Name;
//            ViewBag.UserName = userName;
//            return View();
//        }
//        }
//    }

using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Office.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text;
using System.Text.Json;
using WebAppDemo.Models;
using WebAppDemo.Models.ExcelImportModels;

namespace WebAppDemo.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult UploadExcel()
        {
            var userName = User.Identity?.Name;
            ViewBag.Username = userName;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return Json(new { success = false, message = "Excel dosyası boş." });

            using var stream = new MemoryStream();
            await excelFile.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet?.Dimension == null || worksheet.Dimension.Rows < 2)
                return Json(new { success = false, message = "Excel boş ya da yeterli sayıda kayıt yok." });

            var headers = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                headers.Add(worksheet.Cells[1, col].Text);

            TempData["ExcelFile"] = Convert.ToBase64String(stream.ToArray());

            return Json(new { success = true, headers = headers });
        }

        public async Task<IActionResult> ColumnMappingResult(IFormCollection form)
        {
            //önceki methodda tempdatayı byte gibi karmaşık nesneleri saklayamayacağı için tobase64 yaptık o değer tempexcel 
            var tempExcel = TempData["ExcelFile"] as string;
            //veri yoksa sayfa yenilenmiş olabilir vs. tekrar yükleme sayfasına dön
            if (tempExcel == null)
                return RedirectToAction("UploadExcel");
            //base 64 ten tekrar byte dizisine çeviriyoruz
            byte[] excelBytes = Convert.FromBase64String(tempExcel);
            //byte dizisinde bulunan excelBytes rame kaydediyoruz
            using var stream = new MemoryStream(excelBytes);
            //epplus kütüphanesiyle excel dosyasını açıyoruz
            using var package = new ExcelPackage(stream);

            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet.Dimension == null)
            {
                return Json(new { succees = false, message = "Excel boş ya da yeterli sayıda kayıt yok." });
            }



            var mapping = new Dictionary<string, string>();

            foreach (var key in form.Keys)
            {
                if (key.EndsWith("Column"))
                {
                    var fieldName = key.Replace("Column", "");
                    var excelHeader = form[key];
                    if (!string.IsNullOrEmpty(excelHeader))
                    {
                        mapping[fieldName] = excelHeader;
                    }
                }
            }

            var results = new List<UserImportResult>();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var user = new SignupModel();
                var result = new UserImportResult
                {
                    RowNumber = row,
                    User = user
                };

                foreach (var map in mapping)
                {
                    var alan = map.Key;
                    var excelCol = map.Value;
                    int colIndex = -1;

                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        if (worksheet.Cells[1, col].Text == excelCol)
                        {
                            colIndex = col;
                            break;
                        }
                    }

                    var cellValue = worksheet.Cells[row, colIndex].Text;

                    switch (alan)
                    {
                        case "TCKimlikNumarası":
                            user.IdentityNumber = long.TryParse(cellValue, out var tc) ? tc : 0;
                            break;
                        case "KullanıcıAdı":
                            user.UserName = cellValue;
                            break;
                        case "E-Mail":
                            user.Email = cellValue;
                            break;
                        case "Şifre":
                            user.Password = cellValue;
                            break;
                        case "KullanıcıSeviyesi":
                            user.AuthorityLevel = int.TryParse(cellValue, out var level) ? level : null;
                            break;
                    }
                }

                results.Add(result);
            }

            return View("ImportResult", results); 
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveUsers([FromBody] List<SignupModel> users)
        {
            var client = new HttpClient();
            var errors = new List<string>();

            foreach (var user in users)
            {
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5269/api/Auth/signup", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errMsg = await response.Content.ReadAsStringAsync();
                    errors.Add($"Hata - {user.Email}: {errMsg}");
                }
            }

            if (errors.Count > 0)
                return Json(new { success = false, message = $"Bazı kayıtlar başarısız:\n{string.Join("\n", errors)}" });

            return Json(new { success = true, message = "Tüm kullanıcılar başarıyla kaydedildi." });
        }

        public IActionResult ImportResult()
        {
            var userName = User.Identity?.Name;
            ViewBag.Username = userName;
            return View();
        }
    }
}

