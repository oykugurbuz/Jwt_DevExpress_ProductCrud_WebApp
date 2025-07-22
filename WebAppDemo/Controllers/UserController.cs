using DevExpress.Office.Utils;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text.Json;
using WebAppDemo.Models;

namespace WebAppDemo.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            var userName = User.Identity?.Name; 
            ViewBag.UserName = userName;
            var currentAuthorityLevel = User.Claims.FirstOrDefault(c => c.Type == "AuthorityLevel")?.Value;

            ViewBag.AuthorityLevel = currentAuthorityLevel;
            if (excelFile == null || excelFile.Length == 0) 
            {
                return View();
            }

            var userList = new List<SignupModel>(); 
            var errorUsers = new List<SignupModel>(); 
            using (var stream = new MemoryStream()) 
            {
                await excelFile.CopyToAsync(stream); 
                
                using (var package = new ExcelPackage(stream)) 
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); 

                    if(worksheet == null) 
                    {
                        ViewBag.UploadError = "Excel sayfası bulunamadı!";
                        return View();
                    }
                    if(worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                    {
                        ViewBag.UploadError = "Excel dosyası boş görünüyor. Lütfen en az bir kullanıcı kaydı içeren bir dosya yükleyin.";
                        return View();
                    }
                    int rowCount = worksheet.Dimension.Rows; 

                    for(int row= 2; row <= rowCount; row++) 
                    {
                        var user = new SignupModel 
                        {
                            UserName = worksheet.Cells[row, 1].Text?.Trim(), 
                            IdentityNumber = long.Parse(worksheet.Cells[row, 2].Text?.Trim() ?? "0"),
                            Email = worksheet.Cells[row, 3].Text?.Trim(),
                            Password = worksheet.Cells[row, 4].Text?.Trim()
                        };
                        userList.Add(user); 
                    }

                    using (var httpClient = new HttpClient()) 
                    {
                        httpClient.BaseAddress = new Uri("http://localhost:5269");
                        foreach(var user in userList) 
                        {
                            var json = JsonSerializer.Serialize(user); 
                            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"); 
                                                                                                                 
                            var response = await httpClient.PostAsync("api/Auth/signup/", content); 

                            if(!response.IsSuccessStatusCode)
                            {
                                errorUsers.Add(user);
                                
                            }
                        }

                            
                        }
                    if (errorUsers.Any())
                    {
                        TempData["ErrorUsers"] = JsonSerializer.Serialize(errorUsers);

                        ViewBag.UploadError = $"Toplam {errorUsers.Count} hatalı kayıt var. Lütfen 'Hatalı Kayıtları İndir' butonuyla dosyayı indirin.";
                        return View();
                        
                    }
                    
                    
                    ViewBag.UploadMessage = "Kullanıcı kaydı başarılı!";
                    return View();
                  

                }
            }
        }
        public IActionResult DownloadErrorExcel()
        {
            if (!TempData.ContainsKey("ErrorUsers"))
                return RedirectToAction("UploadExcel");

            var errorUsersJson = TempData["ErrorUsers"] as string;
            if (string.IsNullOrEmpty(errorUsersJson))
                return RedirectToAction("UploadExcel");

            var errorUsers = JsonSerializer.Deserialize<List<SignupModel>>(errorUsersJson);
            var errorExcel = CreateErrorExcel(errorUsers);
            var fileName = $"HatalıKayıtlar_{DateTime.Now: dd.MM.yyyy_HH.mm}.xlsx";

            return File(errorExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        private byte[] CreateErrorExcel(List<SignupModel> errors)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Hatalı Kayıtlar"); 
                worksheet.Cells[1,1].Value = "Kullanıcı Adı";
                worksheet.Cells[1, 2].Value = "Kimlik Numarası";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Parola";
                int row = 2; 
                foreach (var error in errors) 
                {
                    worksheet.Cells[row, 1].Value = error.UserName; 
                    worksheet.Cells[row, 2].Value = error.IdentityNumber;
                    worksheet.Cells[row, 3].Value = error.Email; 
                    worksheet.Cells[row, 4].Value = error.Password; 
                    row++; //bir sonraki satıra geçiyoruz.
                }
                return package.GetAsByteArray(); // Excel paketini byte dizisi olarak döndür
            }


        }

        public IActionResult UserList()
        {
            

            var userList = _context.AppUserInfos.Select(u => new UserList
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                AuthorityLevel = u.AuthorityLevel,
                //IdentityNumber = u.AuthorityLevel == 1 ? u.IdentityNumber : null
                IdentityNumber = u.IdentityNumber


            }).ToList();

            return Json(userList);
        }

    
    }

    
}
