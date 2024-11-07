using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using System.Security.Claims;
using EmployeeManagement.ViewModels;
using AutoMapper;

namespace EmployeeManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public EmployeesController(IMapper mapper, IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        // GET: Employees
        public async Task<IActionResult> Index(EmployeeViewModel employees)
        {
            employees.Employees = await _context.Employees.Include(x => x.Status).ToListAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Gender)
                .Include(e => e.Country)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Bank)
                .Include(e => e.EmploymentTerms)
                .Include(e => e.Disability)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code== "EmploymentTerms"), "Id", "Description");
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code== "DisabilityTypes"), "Id", "Description");
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code=="Gender"), "Id", "Description");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel newemployee, IFormFile employeephoto)
        {
            var employee = new Employee();
            _mapper.Map(newemployee, employee);

            if(employeephoto.Length > 0)
            {
                var fileName = "EmployeePhoto_" + DateTime.Now.ToString("yyyymmddhhmmss") + "_" + employeephoto.FileName;
                var path = _configuration["FileSettings:UploadFolder"]!;
                var filePath = Path.Combine(path, fileName);
                var stream = new FileStream(filePath, FileMode.Create);
                await employeephoto.CopyToAsync(stream);
                employee.Photo = fileName;
            }

            var statusId = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmployeeStatus" && x.Code == "Active").FirstOrDefaultAsync();

            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            employee.CreatedById = "Code Craft";
            employee.CreatedOn = DateTime.Now;
            employee.ModifiedById = employee.CreatedById;
            employee.ModifiedOn = employee.CreatedOn;
            employee.StatusId = statusId.Id;

            ModelState.Remove("CreatedById");
            ModelState.Remove("ModifiedById");
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Key: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }
                _context.Add(employee);
                await _context.SaveChangesAsync(Userid);
                return RedirectToAction(nameof(Index));

            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employee.BankId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmploymentTerms"), "Id", "Description", employee.EmploymentTermsId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "DisabilityTypes"), "Id", "Description", employee.DisabilityId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmploymentTerms"), "Id", "Description");
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "DisabilityTypes"), "Id", "Description");
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewBag.ExistingPhoto = employee.Photo;
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? employeephoto)
        {
            if (employeephoto != null && employeephoto.Length > 0)
            {
                var fileName = "EmployeePhoto_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + employeephoto.FileName;
                var path = _configuration["FileSettings:UploadFolder"]!;
                var filePath = Path.Combine(path, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await employeephoto.CopyToAsync(stream);
                }

                // Update employee photo only if a new photo is uploaded
                employee.Photo = fileName;
            }

            var statusId = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmployeeStatus" && x.Code == "Active").FirstOrDefaultAsync();

            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            employee.CreatedById = "";
            employee.ModifiedById = Userid;
            employee.StatusId = statusId.Id;

            if (id != employee.Id)
            {
                return NotFound();
            }


                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync(Userid);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));


            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employee.BankId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmploymentTerms"), "Id", "Description", employee.EmploymentTermsId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "DisabilityTypes"), "Id", "Description", employee.DisabilityId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Gender)
                .Include(e => e.Country)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Bank)
                .Include(e => e.EmploymentTerms)
                .Include(e => e.Disability)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync(Userid);
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
