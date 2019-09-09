using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace First_part.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorization : AuthorizeAttribute, IAuthorizationFilter
    {

        public string myRole { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userName = context.HttpContext.User.Identity.Name;
            var dbContext = (ApplicationIdentityDbContext)context.HttpContext.RequestServices.GetService(typeof(ApplicationIdentityDbContext));
            var dbUser = dbContext.Users
              .FirstOrDefault(u => u.UserName == userName);

            var userRoles = dbContext.UserRoles.Where(ur => ur.UserId == dbUser.Id).ToList();
            var roles = dbContext.Roles.Where(r => userRoles.Any(ur => ur.RoleId == r.Id)).ToList();


            var rolePermissions = dbContext.RolePermissions
                .Where(rp => roles.Any(r => r.Id == rp.RoleId)).ToList();

            var permission = dbContext.Permissions.Where(p => rolePermissions.Any(rp => rp.PermissionId == p.Id))
                .ToList();

           

            var hasClaim = permission.Any(p => p.Name == myRole);

            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }

        }//end of method


    }//end of class

}//end of namespace
