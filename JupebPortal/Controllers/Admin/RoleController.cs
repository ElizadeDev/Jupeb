using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JupebPortal.Data;
using JupebPortal.Models;
using Microsoft.AspNetCore.Identity;
using JupebPortal.ViewModels;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace JupebPortal.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public RoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: RoleModels
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // GET: RoleModels/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var roleModel = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModel == null)
            {
                return NotFound();
            }

            return View(roleModel);
        }

        // GET: RoleModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RoleModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(role);
        }

        // GET: RoleModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: RoleModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = await _roleManager.FindByIdAsync(role.Id);

                if (existingRole == null)
                {
                    return NotFound();
                }

                existingRole.Name = role.Name;
                var result = await _roleManager.UpdateAsync(existingRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(role);
        }

        //GET: RoleModels/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: RoleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("Index");
        }

        private bool RoleModelExists(string id)
        {
            return (_context.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
        }






        public IActionResult AssignRole()
        {
            var model = new AssignRoleViewModel
            {
                Users = _userManager.Users,
                Roles = _roleManager.Roles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (user == null || role == null)
            {
                ModelState.AddModelError("", "Invalid user or role.");
                return View(model);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(role.Name))
            {
                ModelState.AddModelError("", "The user is already assigned to this role.");
                return RedirectToAction("Index");
            }
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                
                return RedirectToAction("Index", "Home"); // Redirect to a relevant page after successfully assigning the role
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }


        public IActionResult UserRoles()
        {
            var users = _userManager.Users.Select(c => new UserRolesViewModel()
            {
                UserId = c.Id,
                Name = c.Surname + " " + c.FirstName,
                Email = c.Email,
                Role = string.Join(", ", _userManager.GetRolesAsync(c).Result.ToArray())
            }).ToList();
            return View(users);
        }

        public async Task<IActionResult> EditUserRoleAsync(string? userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (userId == null || _context.UserRoles == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.UserId = userId;
            ViewBag.Username = user.Surname + " " + user.FirstName;
            ViewBag.Email = user.Email;
            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var message = "";
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Handle the case where the user does not exist
                message = "Error: No user selected.";
                TempData["error"] = message;
                return BadRequest(message);
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
            {
                // Handle the case where the user is not in the specified role
                message = "Error: The user does not have the specified role.";
                TempData["error"] = message;
                return BadRequest(message);
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                // Role successfully removed
                message = "Role removed successfully.";
                TempData["success"] = message;
                return Ok(message);
            }
            else
            {
                // Handle the case where the removal of role failed
                message = "Error: Role removal failed.";
                TempData["error"] = message;
                return BadRequest(message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName, string userId)
        {
            var message = "";

            if (roleName == null)
            {
                message = "No role supplied.";
                TempData["error"] = message;
                return BadRequest(message);
            }

            var user = await _userManager.FindByIdAsync(userId);
            var role = await _roleManager.FindByNameAsync(roleName);

            if (user == null || role == null)
            {
                message = "Invalid user or role.";
                TempData["error"] = message;
                return BadRequest(message);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(role.Name))
            {
                message = "The user is already assigned to this role.";
                TempData["error"] = message;
                return BadRequest(message);
            }
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                message = "Role successfully assigned to user";
                TempData["success"] = message;
                return Ok(new { message, roleName, userId });
            }
            else
            {
                message = "Error adding role";
                TempData["error"] = message;
                return BadRequest(message);
            }
        }
    }
}

