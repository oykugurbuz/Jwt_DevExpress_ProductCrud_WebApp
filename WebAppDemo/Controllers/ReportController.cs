using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppDemo.Models.ReportModels;
using WebAppDemo.Models;
using WebAppDemo.Report;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET ile query parametreleri alacak
    [HttpGet]
    [HasPermission("Report.Export")]
    public IActionResult ExportPdf([FromQuery] ReportFilterModel model)
    {
        if (model == null)
        {
            return BadRequest("Boş veri");
        }

        var products = _context.Products.Include(p => p.Category).AsQueryable();

        if (model.MinPrice.HasValue)
            products = products.Where(p => p.Price >= model.MinPrice.Value);
        if (model.MaxPrice.HasValue)
            products = products.Where(p => p.Price <= model.MaxPrice.Value);
        if (model.StartDate.HasValue)
            products = products.Where(p => p.OpenDate >= model.StartDate.Value);
        if (model.EndDate.HasValue)
            products = products.Where(p => p.OpenDate <= model.EndDate.Value);
        if (model.CategoryId.HasValue)
            products = products.Where(p => p.CategoryId == model.CategoryId.Value);

        var data = products.Select(p => new ReportViewModel
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.CategoryName,
            Price = p.Price,
            OpenDate = p.OpenDate,
            Description = p.Description
        }).ToList();

        var report = new ProductReport
        {
            DataSource = data,
            DisplayName = "Rapor"
        };

        using var ms = new MemoryStream();
        report.CreateDocument();
        report.ExportToPdf(ms);
        ms.Position = 0;

        return File(ms.ToArray(), "application/pdf");
    }

    [HttpPost]
   
    public IActionResult ExportPdfPost([FromBody] ReportFilterModel model)
    {
        return ExportPdf(model);
    }

    public IActionResult ReportPage()
    {
        var username = User.Identity?.Name;
        ViewBag.Username = username;
        var categories = _context.Categories
            .Select(c => new { categoryId = c.CategoryId, categoryName = c.CategoryName })
            .ToList();
        ViewBag.Categories = categories;

        return View();
    }
}