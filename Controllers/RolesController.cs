using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model)
        {
            IdentityRole role = new IdentityRole();
            role.Name = model.RoleName;

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var role = new RoleViewModel();
            var result = await _roleManager.FindByIdAsync(id);
            role.RoleName = result.Name;
            role.Id = result.Id;
            return View(role);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, RoleViewModel model)
        {
            var checkifexist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!checkifexist)
            {
                var result = await _roleManager.FindByIdAsync(id);
                result.Name = model.RoleName;

                var finalResult = await _roleManager.UpdateAsync(result);

                if (finalResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> UserRights(string id)
        {
            var tasks = new ProfileViewModel();
            tasks.RoleId = id;
            tasks.Profiles = await _context.SystemProfiles
                .Include(s => s.Profile)
                .Include("Children.Children.Children")
                .OrderBy(x => x.Order)
                .ToListAsync();

            tasks.RolesRightsIds = await _context.RoleProfiles.Where(x => x.RoleId == id).Select(r => r.TaskId).ToListAsync();
            return View(tasks);
        }

        [HttpPost]
        public async Task<ActionResult> UserGroupRights(string id, ProfileViewModel vm)
        {
            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var allRights = await _context.RoleProfiles.Where(x => x.RoleId == id).ToListAsync();
            _context.RoleProfiles.RemoveRange(allRights);
            await _context.SaveChangesAsync(Userid);
            foreach (var taskId in vm.Ids)
            {
                var role = new RoleProfile
                {
                    TaskId = taskId,
                    RoleId = id,
                };

                _context.RoleProfiles.Add(role);
                await _context.SaveChangesAsync(Userid);
            }
            return RedirectToAction("Index");
        }
    }
}
