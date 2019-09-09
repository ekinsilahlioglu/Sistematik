using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using First_part.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace First_part.Controllers
{
    public class HomeController : Controller
    {

        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signManager;
        private IPasswordValidator<ApplicationUser> passwordValidator;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        public ApplicationIdentityDbContext context { get; set; }
        

        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        public HomeController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signManager, IPasswordValidator<ApplicationUser> _passwordValidator, IPasswordHasher<ApplicationUser> _passwordHasher)
        {
            userManager = _userManager;
            signManager = _signManager;
            passwordValidator = _passwordValidator;
            passwordHasher = _passwordHasher;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //sign up part
        [HttpPost]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                

                var result = await userManager.CreateAsync(user, model.Password);
                
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
            return View(model);
        }

        //login to system
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await signManager.SignOutAsync();
                    var result = await signManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        await signManager.SignInAsync(user, false);

                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return Redirect("Page");
                        }
                        if (await userManager.IsInRoleAsync(user, "operator"))
                        {
                            return Redirect("Page2");
                        }
                        if (await userManager.IsInRoleAsync(user, "Personel"))
                        {
                            return Redirect("Page3");
                        }

                    }

                }

                ModelState.AddModelError("Email", "Invalid Email or Password");
            }
            
            return View(model);
        }   


        //admin page
        [MyAuthorization(myRole = "Page")]
        public IActionResult Page()
        {
            return View();
        }

        //operator page
        [MyAuthorization(myRole = "Page2")]
        public IActionResult Page2()
        {
            return View();
        }

        //personel page
        [MyAuthorization(myRole = "Page3")]
        public IActionResult Page3()
        {
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await signManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        
        public IActionResult UserL()
        {
            return View(userManager.Users);
        }
       


        //delete user
        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if(user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserL");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found.");
            }

            return View("UserL",userManager.Users);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("UserL");
            }
        }

        //update user information
        [HttpPost]
        public async Task<IActionResult> Update(string id, string password, string email)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, password);
                    }
                    else
                    {
                        foreach (var item in validPass.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                if (validPass.Succeeded)
                {
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserL");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found.");
            }
            return View(user);
        }

    }//end of class
}//end of namespace
