using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Models;

namespace WebAppDemo.Controllers
{
    [Authorize]
    public class AuthorizeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorizeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HasPermission("Product.Read")]
        public IActionResult SecretPage()
        {
            var username = User.Identity?.Name;
            ViewBag.Username = username;
            ViewBag.Categories = _context.Categories.Select(c => new { categoryId = c.CategoryId, categoryName = c.CategoryName }).ToList();

            var products = _context.Products
           .Include(p => p.Category)
           .Select(p => new ProductViewModel
           {
               ProductId = p.ProductId,
               ProductName = p.ProductName,
               Price = p.Price,
               CategoryName = p.Category.CategoryName,
               Description = p.Description,
               CategoryId = p.CategoryId
           })
             .ToList();

            return View(products);
        }

        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [HasPermission("Product.Create")]
        public IActionResult CreateProduct([FromBody] ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == model.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Geçersiz kategori seçimi.");
                return BadRequest(ModelState);
            }

            var newProduct = new Product
            {
                ProductName = model.ProductName,
                Price = model.Price,
                Description = model.Description,
                IsActive = true,
                CategoryId = model.CategoryId
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.ToList();
            return Json(product);
        }

        [HttpPost]
        [HasPermission("Product.Update")]
        public IActionResult EditProduct([FromBody] Product model)
        {

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == model.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Geçersiz kategori seçimi.");
                return BadRequest(ModelState);
            }
            var product = _context.Products.FirstOrDefault(p => p.ProductId == model.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;
            product.IsActive = true;
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [HasPermission("Product.Delete")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IActionResult SearchProduct(string searchTerm)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm));
            }
            var products = query.Select (p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price= p.Price,
                    Description = p.Description,
                    CategoryName = p.Category.CategoryName,
                    CategoryId = p.CategoryId
                }).ToList();

            return Json(products);
        }
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.Select(c => new
            {
                categoryId = c.CategoryId,
                categoryName = c.CategoryName
            }).ToList();
            return Json(categories);


        }

        [HttpPost]
        [HasPermission("Category.Create")]
        public IActionResult CreateCategory([FromBody] CategoryViewModel model)
        {
                if (string.IsNullOrWhiteSpace(model.CategoryName))
                {
                    return BadRequest("Boş bırakamazsınız!");
                }
             
            
               if (_context.Categories.Any(c => c.CategoryName == model.CategoryName.Trim())) //trim baştaki ve sondaki boşlukları temizler
            {
                return BadRequest("Bu kategori zaten bulunmakta!");
            }

            var newCategory = new Category
            {
                CategoryName = model.CategoryName
            };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return Ok();
        }

        [HasPermission("Category.Read")]
        public IActionResult CategoryPage()
        {
            var username = User.Identity?.Name;
            ViewBag.Username = username;

            ViewBag.Categories = _context.Categories.Select(c => new { categoryId = c.CategoryId, categoryName = c.CategoryName }).ToList();
            var categories = _context.Categories.Select(c => new CategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            })
                .ToList();

            return View(categories);
        }

        [HttpPost]
        [HasPermission("Category.Delete")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if(category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok();
        }

        
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if(category == null)
            {
                return NotFound();
            }
            return Json(category);
           
        }
        [HttpPost]
        [HasPermission("Category.Update")]
        public IActionResult EditCategory([FromBody]Category model)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == model.CategoryId);
            if(category == null)
            {
                return NotFound();
            }
            category.CategoryName = model.CategoryName;
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("login", "auth");
        }
    }
}
