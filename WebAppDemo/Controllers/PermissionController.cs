using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebAppDemo.Models;
using WebAppDemo.Models.PermissionModels;


namespace WebAppDemo.Controllers
{
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermissionController(ApplicationDbContext context)
        {
            _context = context;
        }


        private string GetCurrentUserName()
        {
            return User.Identity?.Name ?? string.Empty;
        }


        private AppUserInfo GetCurrentUser()
        {
            var username = GetCurrentUserName();
            return _context.AppUserInfos.FirstOrDefault(u => u.UserName == username);
        }


        public IActionResult Assign()
        {
            ViewBag.Username = GetCurrentUserName();
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return Unauthorized();

            var users = _context.AppUserInfos
                .Where(u => u.UserName == currentUser.UserName || u.AuthorityLevel > currentUser.AuthorityLevel) 
                .Select(u => new UserDropdownItem
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .OrderBy(u => u.UserName)
                .ToList(); 


            var modules = _context.Modules
                .Include(m => m.Permissions)
                .Select(m => new ModuleWithPermissions
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    Permissions = m.Permissions.Select(p => new PermissionItem
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName,
                        IsAssigned = false
                    }).ToList()
                })
                .ToList();

            var vm = new PermissionAssignViewModel
            {
                Users = users,
                Modules = modules
            };

            return View(vm);
        }


        [HttpGet]
        public IActionResult GetUserPermissions(int userId)
        {
            var currentUser = GetCurrentUser();
            var targetUser = _context.AppUserInfos.FirstOrDefault(u => u.Id == userId);

            if (currentUser == null || targetUser == null)
                return NotFound();

            if (targetUser.Id != currentUser.Id && targetUser.AuthorityLevel <= currentUser.AuthorityLevel)
                return Forbid(); //o veya alt yetkisi değilse izin verme 

            var assignedPermissionIds = _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToList();

            return Json(assignedPermissionIds);
        }


        [HttpPost]
        public IActionResult Assign(PermissionAssignViewModel vm)
        {
            if (vm.SelectedUserId > 0)
            {
                var currentUser = GetCurrentUser();
                var targetUser = _context.AppUserInfos.FirstOrDefault(u => u.Id == vm.SelectedUserId);

                if (currentUser == null || targetUser == null)
                    return NotFound();

                if (targetUser.Id != currentUser.Id && targetUser.AuthorityLevel <= currentUser.AuthorityLevel)
                    return Forbid();


                var oldUserPermissions = _context.UserPermissions.Where(up => up.UserId == vm.SelectedUserId);
                _context.UserPermissions.RemoveRange(oldUserPermissions);


                foreach (var pid in vm.AssignedPermissionIds)
                {
                    _context.UserPermissions.Add(new UserPermission
                    {
                        UserId = vm.SelectedUserId,
                        PermissionId = pid,
                        GivenByUserId = currentUser.Id,
                        GivenDate = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                _context.SaveChanges();

                TempData["Success"] = "Yetkiler başarıyla güncellendi.";
                return RedirectToAction(nameof(Assign));
            }
            TempData["Error"] = "Yetki güncellenemedi.Lütfen bir kullanıcı seçin.";
            return RedirectToAction(nameof(Assign));

        }
       
    }
}

