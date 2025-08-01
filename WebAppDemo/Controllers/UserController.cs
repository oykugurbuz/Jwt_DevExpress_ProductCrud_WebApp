using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.CodeParser;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.Office.Utils;
using DevExpress.XtraGauges.Core.Styles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using OfficeOpenXml;
using System.Text;
using System.Text.Json;
using WebAppDemo.Models;
using WebAppDemo.Models.ExcelImportModels;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

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
            var userName = User.Identity?.Name;
            ViewBag.Username = userName;
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
            var results = new List<UserImportResult>();
            using var client = new HttpClient();

            foreach (var user in users)
            {
                var result = new UserImportResult
                {
                    User = user
                };

                try
                {
                    var json = JsonSerializer.Serialize(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("http://localhost:5269/api/Auth/signup", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();

                        try
                        {
                            var errorDoc = JsonDocument.Parse(responseBody);

                            // 1. Validation hataları
                            if (errorDoc.RootElement.TryGetProperty("errors", out var errorsElement))
                            {
                                var errorMessages = new List<string>();
                                foreach (var property in errorsElement.EnumerateObject())
                                {
                                    foreach (var message in property.Value.EnumerateArray())
                                    {
                                        errorMessages.Add(message.GetString());
                                    }
                                }

                                result.Message = string.Join(" | ", errorMessages);
                            }
                            // 2. Tek bir hata mesajı varsa
                            else if (errorDoc.RootElement.TryGetProperty("message", out var messageElement))
                            {
                                result.Message = messageElement.GetString();
                            }
                            // 3. Bilinmeyen hata
                            else
                            {
                                result.Message = responseBody;
                            }
                        }
                        catch
                        {
                            result.Message = responseBody;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }

                results.Add(result);
            }
            return PartialView("_ImportResultTable", results); // sadece <tr> satırları dönüyor

           
        }

        public IActionResult ImportResult()
        {
            var userName = User.Identity?.Name;
            ViewBag.Username = userName;
            return View();
        }
    }
}

