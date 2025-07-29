using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebAppDemo.Models;
using WebAppDemo.Models.PermissionModels;


namespace WebAppDemo.Controllers
{
    [Authorize]
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
            //hedef kullanıcıya ait aktif izinler 
            var assignedPermissionIds = _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive)
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
                var targetUser = _context.AppUserInfos.FirstOrDefault(tu => tu.Id == vm.SelectedUserId);

                if (currentUser == null || targetUser == null)

                    return NotFound();
                if (currentUser != targetUser && currentUser.AuthorityLevel >= targetUser.AuthorityLevel)
                {
                    return Forbid();
                }
                //aktif olan userpermissonslar listesi
                var oldPermission = _context.UserPermissions.Where(up => up.UserId == vm.SelectedUserId && up.IsActive).ToList();

                //artık aktifin içinde olmayan(seçilmemiş) izinleri pasif yap

                foreach (var oldp in oldPermission)
                {
                    if (!vm.AssignedPermissionIds.Contains(oldp.PermissionId))
                    {
                        oldp.IsActive = false;
                        oldp.RevokedByUserId = currentUser.Id;
                        oldp.RevokedDate = DateTime.UtcNow;
                    }
                }

                // Yeni seçilen ancak önceden atanmamış izinleri
                foreach (var pid in vm.AssignedPermissionIds)
                {
                    bool alreadyAssigned = oldPermission.Any(up => up.PermissionId == pid && up.IsActive); //zaten atanmış izinler
                    //önceden atanmış olmayan izinleri userpermisson tablosuna ekler.
                    if (!alreadyAssigned)
                    {
                        _context.UserPermissions.Add(new UserPermission
                        {
                            UserId = vm.SelectedUserId,
                            PermissionId = pid,
                            GivenByUserId = currentUser.Id,
                            GivenDate = DateTime.UtcNow,
                            IsActive = true,
                        });
                    }


                }
                _context.SaveChanges();
                TempData["Success"] = "Yetkiler başarıyla güncellendi.";
                return RedirectToAction(nameof(Assign));
            }
            TempData["Error"] = "Yetkiler güncellenemedi. Lütfen bir kullanıcı seçin";
            return RedirectToAction(nameof(Assign));
        }
        public IActionResult UserPermissionHistory()
        {
            var username = GetCurrentUserName();
            ViewBag.Username = username;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FilterUserPermissions([FromBody] UserPermissionFilterModel filter)
        {
            if(filter == null)
            {
                return BadRequest("filtre null");
            }
            var query = _context.UserPermissions
                .Include(up => up.User)
                .Include(up => up.Permission).ThenInclude(p => p.Module)
                .Include(up => up.GivenByUser)
                .Include(up => up.RevokedByUser).AsQueryable();

            if (!string.IsNullOrEmpty(filter.UserName))
            {
                query = query.Where(up => up.User.UserName.Contains(filter.UserName));
            }

            if (!string.IsNullOrEmpty(filter.ModuleName))
            {
                query = query.Where(up => up.Permission.Module.ModuleName.Contains(filter.ModuleName));
            }

            if (!string.IsNullOrEmpty(filter.PermissionName))
            {
                query = query.Where(up => up.Permission.PermissionName.Contains(filter.PermissionName));
            }
            if (!string.IsNullOrEmpty(filter.GivenBy))
            {
                query = query.Where(up => up.GivenByUser.UserName.Contains(filter.GivenBy));
            }

            if (filter.GivenDateStart.HasValue)
            {
                query = query.Where(up => up.GivenDate >= filter.GivenDateStart.Value);
            }
            if (filter.GivenDateEnd.HasValue)
            {
                query = query.Where(up => up.GivenDate <= filter.GivenDateEnd.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(up => up.IsActive == filter.IsActive.Value);
            }
            //eğer aktiflik false ise yetkiyi iptal eden kullanıcı ve iptal etme tarihi filtrelenir.
            if (filter.IsActive == false)
            {
                if (!string.IsNullOrEmpty(filter.RevokedByUser))
                {
                    query = query.Where(up => up.RevokedByUser.UserName.Contains(filter.RevokedByUser));
                }
                if (filter.RevokedDateStart.HasValue)
                {
                    query = query.Where(up => up.RevokedDate >= filter.RevokedDateStart.Value);

                }
                if (filter.RevokedDateEnd.HasValue)
                {
                    query = query.Where(up => up.RevokedDate <= filter.RevokedDateEnd.Value);
                }
            }

            var result = await query.Select(up => new UserPermissionHistoryViewModel
            {
                User = up.User.UserName,
                Module = up.Permission.Module.ModuleName,
                Permission = up.Permission.PermissionName,
                GivenBy = up.GivenByUser.UserName,
                GivenDate = up.GivenDate,
                IsActive = up.IsActive,
                RevokedByUser = up.RevokedByUser != null ? up.RevokedByUser.UserName : null,
                RevokedDate = up.RevokedDate
            }).OrderBy(up => up.User).ToListAsync();
            return Json(result);

        }

       
        public IActionResult GetUserList()
        {
            var users = _context.AppUserInfos.Select(u => new { Value = u.UserName, Text = u.UserName }).ToList();
            return Json(users);
        }

        public IActionResult GetModuleList()
        {
            var modules = _context.Permissions.Select(p => p.Module.ModuleName).Distinct().Select(name => new { Value = name, Text = name }).ToList();
            return Json(modules);
        }

        //modüle göre izinler
        public IActionResult GetPermissionListByModule(string moduleName)
        {
            var permission = 
                _context.Permissions.Where(p=> p.Module.ModuleName == moduleName).Select(p=> new
                {
                    Value = p.PermissionName,
                    Text = p.PermissionName
                }).ToList();
            return Json(permission);
        }
    }
}