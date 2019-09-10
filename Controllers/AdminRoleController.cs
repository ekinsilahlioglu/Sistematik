using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First_part.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace First_part.Controllers
{
    
    public class AdminRoleController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        private ApplicationIdentityDbContext context { get; set; }
        public AdminRoleController(ApplicationIdentityDbContext _context,RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            context = _context;
        }

       
        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }

     
        public IActionResult Create()
        {
            return View();
        }


        //create roles
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {

            if (ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }

            return View(name);
            
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            var members = new List<ApplicationUser>();
            var nonmembers = new List<ApplicationUser>();
            foreach (var user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user,role.Name)?members:nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };

            return View(model);
        }

        //add or delete a person from that role
        [HttpPost]
        public async Task<IActionResult> Edit(RoleEditModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[]{ })
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                foreach (var userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                   
                }
             
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", model.RoleId);
        }


        //delete roles
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role !=null)
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
                
            }
            return RedirectToAction("Index");
        }


        private MyAuthorization myAuthorization = new MyAuthorization();



        public IActionResult AutUpdate(string id)
        {

            var selectedPermissions = context.RolePermissions.Where(rp => rp.RoleId == id)
                .Select(rp => rp.PermissionId).ToList();

            ViewBag.selectedPermission = selectedPermissions;

            var permissions = context.Permissions.ToList();

            return View(permissions);
        }






    }//end of class
}//end of namespace
