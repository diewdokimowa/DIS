using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Lab_34.Data;
using Lab_34.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab_34.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        public IActionResult Index([FromServices] UserManager<ApplicationUser> manager)
        {
            return View(manager);
        }

        [HttpGet]
        public IActionResult Edit(string id, [FromServices] ApplicationDbContext db)
        {
            ApplicationUser user;
            if (id == null)
            {
                user = new ApplicationUser();
            }
            else
            {
                user = db.ApplicationUser.Find(id);
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user, [FromServices] ApplicationDbContext db, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] EmailSender emailSender)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var _user  = db.ApplicationUser.Find(user.Id);
            if (_user == null)
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.EmailConfirmed = true;
                await userManager.CreateAsync(user);
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var urlEncode = HttpUtility.UrlEncode(code);
                var callbackUrl = $"{Request.Scheme}://{Request.Host.Value}/Identity/Account/ResetPassword?userId={user.Id}&code={urlEncode}";
                await emailSender.SendEmailAsync(user.Email, "Активирование", $"Активируйте запись: {callbackUrl}");
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            _user.FirstName = user.FirstName;
            _user.MiddleName = user.MiddleName;
            _user.LastName = user.LastName;
            _user.UserName = user.UserName;
            _user.Email = user.Email;
            _user.EmailConfirmed = user.EmailConfirmed;
            await userManager.UpdateAsync(_user);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(string id, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ToggleAdmin(string id, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await userManager.FindByIdAsync(id);
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }
    }
}