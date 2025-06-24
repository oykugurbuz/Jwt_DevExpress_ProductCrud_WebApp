using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using WebAppDemo.Models;

public class HasPermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _requiredPermission;

   
    public HasPermissionAttribute(string requiredPermission)
    {
        _requiredPermission = requiredPermission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        
        var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

        var userName = context.HttpContext.User.Identity?.Name;

        
        if (string.IsNullOrEmpty(userName) || dbContext == null)
        {
            context.Result = new JsonResult(new { error = "Kullanıcı doğrulanamadı." }) { StatusCode = 403 };
            return;
        }

        
        var user = dbContext.AppUserInfos.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
        {
            context.Result = new JsonResult(new { error = "Kullanıcı sistemde bulunamadı." }) { StatusCode = 403 };
            return;
        }

       
        var userPermissionIds = dbContext.UserPermissions
            .Where(p => p.UserId == user.Id && p.IsActive)
            .Select(p => p.PermissionId)
            .ToList();

       // Product.Create gibi gelen yetki ifadesi "Product" ve "Create" olarak ayrıştırılır.
        var parts = _requiredPermission.Split('.');
        if (parts.Length != 2)
        {
            context.Result = new JsonResult(new { error = $"Yetki tanımı hatalı: {_requiredPermission}" }) { StatusCode = 403 };
            return;
        }

        var moduleName = parts[0];
        var actionName = parts[1];

      
        var matchedPermission = dbContext.Permissions
            .FirstOrDefault(p => p.PermissionName == actionName && p.Module.ModuleName == moduleName);

        
        if (matchedPermission == null)
        {
            context.Result = new JsonResult(new { error = $"Tanımlı olmayan yetki: {_requiredPermission}" }) { StatusCode = 403 };
            return;
        }

        if (!userPermissionIds.Contains(matchedPermission.PermissionId))
        {
            context.Result = new JsonResult(new { error = $"Bu işlem için yetkiniz yok: {_requiredPermission}" }) { StatusCode = 403 };
            return;
        }

      
    }
}
