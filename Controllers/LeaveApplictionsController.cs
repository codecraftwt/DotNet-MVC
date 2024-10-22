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
using EmployeeManagement.Migrations;

namespace EmployeeManagement.Controllers
{
    public class LeaveApplictionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LeaveApplictionsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: LeaveApplictions
        public async Task<IActionResult> Index()
        {
            var awaitingstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "AwaitingApproval").FirstOrDefault();

            var applicationDbContext = _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == awaitingstatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ApprovedLeaveApplications()
        {
            var approvedstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Approved").FirstOrDefault();

            var applicationDbContext = _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == approvedstatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> RejectedLeaveApplications()
        {
            var rejectedstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Rejected").FirstOrDefault();

            var applicationDbContext = _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == rejectedstatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LeaveApplictions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LeaveAppliction = await _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (LeaveAppliction == null)
            {
                return NotFound();
            }

            return View(LeaveAppliction);
        }

        // GET: LeaveApplictions/Create
        public IActionResult Create()
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ApproveLeave(int? id)
        {
            var LeaveAppliction = await _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (LeaveAppliction == null)
            {
                return NotFound();
            }

            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View(LeaveAppliction);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveLeave(LeaveAppliction leave)
        {
            var approvedstatus = _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Approved")
                .FirstOrDefault();

            var adjustmentType = _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveAdjustment" && y.Code == "Negative")
                .FirstOrDefault();

            var leaveAppliction = await _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == leave.Id);

            if (leaveAppliction == null)
            {
                return NotFound();
            }
            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            leaveAppliction.ApprovedOn = DateTime.Now;
            leaveAppliction.ApprovedById = Userid;
            leaveAppliction.CreatedById = "";
            leaveAppliction.StatusId = approvedstatus!.Id;
            leaveAppliction.ApprovalNotes = leave.ApprovalNotes;
            _context.Update(leaveAppliction);
            await _context.SaveChangesAsync(Userid);

            var adjustment = new LeaveAdjustmentEntry
            {
                EmployeeId = leaveAppliction.EmployeeId,
                CreatedById = leaveAppliction.CreatedById = "",
                ModifiedById = leaveAppliction.ModifiedById = "",
                NoOfDays = leaveAppliction.NoOfDays,
                LeaveStartDate = leaveAppliction.StartDate,
                LeaveEndDate = leaveAppliction.EndDate,
                AdjustmentDescription = "Leave Taken Negative Adjustment",
                LeavePeriodId = 2,
                LeaveAdjustmentDate = DateTime.Now,
                AdjustmentTypeId = adjustmentType.Id,
            };
            _context.Add(adjustment);
            await _context.SaveChangesAsync(Userid);

            var employee = await _context.Employees.FindAsync(leaveAppliction.EmployeeId);
            employee.LeaveOutstandingBalance =(employee.AllocatedLeaveDays - leaveAppliction.NoOfDays);
            _context.Update(employee);
            await _context.SaveChangesAsync(Userid);

            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RejectLeave(int? id)
        {
            var LeaveAppliction = await _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (LeaveAppliction == null)
            {
                return NotFound();
            }

            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View(LeaveAppliction);
        }

        [HttpPost]
        public async Task<IActionResult> RejectLeave(LeaveAppliction leave)
        {

            var rejectedstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Rejected").FirstOrDefault();

            var LeaveAppliction = await _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == leave.Id);

            if (LeaveAppliction == null)
            {
                return NotFound();
            }

            LeaveAppliction.ApprovedOn = DateTime.Now;
            LeaveAppliction.ApprovedById = "Code Craft";
            LeaveAppliction.StatusId = rejectedstatus!.Id;
            LeaveAppliction.ApprovalNotes = leave.ApprovalNotes;

            _context.Update(LeaveAppliction);
            await _context.SaveChangesAsync();

            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveApplictions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveAppliction LeaveAppliction, IFormFile leaveattachment)
        {
            if (leaveattachment.Length > 0)
            {
                var fileName = "LeaveAttachmemt_" + DateTime.Now.ToString("yyyymmddhhmmss") + "_" + leaveattachment.FileName;
                var path = _configuration["FileSettings:UploadFolder"]!;
                var filePath = Path.Combine(path, fileName);
                var stream = new FileStream(filePath, FileMode.Create);
                await leaveattachment.CopyToAsync(stream);
                LeaveAppliction.Attachment = fileName;
            }

            var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.Code == "AwaitingApproval" && y.SystemCode.Code == "LeaveApprovalStatus")
                .FirstOrDefaultAsync();

            if (pendingStatus == null)
            {
                ModelState.AddModelError("StatusId", "Pending status not found.");
                return View(LeaveAppliction);
            }

            // Set the required properties
            LeaveAppliction.CreatedById = "Code Craft";
            LeaveAppliction.CreatedOn = DateTime.Now;
            LeaveAppliction.ModifiedById = LeaveAppliction.CreatedById;
            LeaveAppliction.ModifiedOn = LeaveAppliction.CreatedOn;
            LeaveAppliction.ApprovedById = "Code";
            LeaveAppliction.StatusId = pendingStatus.Id;

            // Populate related entities
            LeaveAppliction.Employee = await _context.Employees.FindAsync(LeaveAppliction.EmployeeId);
            LeaveAppliction.Duration = await _context.SystemCodeDetails.FindAsync(LeaveAppliction.DurationId);
            LeaveAppliction.LeaveType = await _context.LeaveTypes.FindAsync(LeaveAppliction.LeaveTypeId);

            // Check if the foreign keys are valid
            if (LeaveAppliction.Employee == null)
            {
                ModelState.AddModelError("EmployeeId", "Employee not found.");
            }
            if (LeaveAppliction.Duration == null)
            {
                ModelState.AddModelError("DurationId", "Duration not found.");
            }
            if (LeaveAppliction.LeaveType == null)
            {
                ModelState.AddModelError("LeaveTypeId", "Leave Type not found.");
            }

            // Remove unnecessary model state entries
            ModelState.Remove("CreatedById");
            ModelState.Remove("ModifiedById");
            ModelState.Remove("ApprovedById");

            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                _context.LeaveApplictions.Add(LeaveAppliction);
                await _context.SaveChangesAsync(Userid);
                return RedirectToAction(nameof(Index));
            }

            // Populate ViewData for the dropdowns in case of an error
            ViewData["DurationId"] = new SelectList(await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration")
                .ToListAsync(), "Id", "Description", LeaveAppliction.DurationId);

            ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", LeaveAppliction.EmployeeId);

            ViewData["LeaveTypeId"] = new SelectList(await _context.LeaveTypes.ToListAsync(), "Id", "Name", LeaveAppliction.LeaveTypeId);

            return View(LeaveAppliction);
        }

        // GET: LeaveApplictions/Edit/5
        // GET: LeaveApplictions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LeaveAppliction = await _context.LeaveApplictions.FindAsync(id);
            if (LeaveAppliction == null)
            {
                return NotFound();
            }

            // Populate ViewData for dropdowns
            ViewData["DurationId"] = new SelectList(await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration")
                .ToListAsync(), "Id", "Description", LeaveAppliction.DurationId);

            ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", LeaveAppliction.EmployeeId);

            ViewData["LeaveTypeId"] = new SelectList(await _context.LeaveTypes.ToListAsync(), "Id", "Name", LeaveAppliction.LeaveTypeId);

            return View(LeaveAppliction);
        }

        // POST: LeaveApplictions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveAppliction leaveAppliction)
        {
            if (id != leaveAppliction.Id)
            {
                return NotFound();
            }

            // Fetch the pending status
            var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.Code == "Pending" && y.SystemCode.Code == "LeaveApprovalStatus")
                .FirstOrDefaultAsync();

            if (pendingStatus == null)
            {
                ModelState.AddModelError("StatusId", "Pending status not found.");
                return View(leaveAppliction);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    leaveAppliction.ModifiedOn = DateTime.Now;
                    leaveAppliction.ModifiedById = "Code Craft";
                    leaveAppliction.StatusId = pendingStatus.Id;

                    _context.Entry(leaveAppliction).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApplictionExists(leaveAppliction.Id))
                    {
                        return NotFound();
                    }
                    // Optionally log the exception here
                    ModelState.AddModelError("", "Unable to save changes. The leave application was modified by another user.");
                }
                catch (Exception ex)
                {
                    // Log the exception (ex) here for further analysis
                    ModelState.AddModelError("", "An error occurred while updating the application.");
                }
            }

            // Re-populate dropdowns if the model state is not valid
            await PopulateDropdowns(leaveAppliction);

            return View(leaveAppliction);
        }

        private async Task PopulateDropdowns(LeaveAppliction leaveAppliction)
        {
            ViewData["DurationId"] = new SelectList(await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveDuration")
                .ToListAsync(), "Id", "Description", leaveAppliction.DurationId);

            ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", leaveAppliction.EmployeeId);

            ViewData["LeaveTypeId"] = new SelectList(await _context.LeaveTypes.ToListAsync(), "Id", "Name", leaveAppliction.LeaveTypeId);
        }

        // GET: LeaveApplictions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LeaveAppliction = await _context.LeaveApplictions
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (LeaveAppliction == null)
            {
                return NotFound();
            }

            return View(LeaveAppliction);
        }

        // POST: LeaveApplictions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var LeaveAppliction = await _context.LeaveApplictions.FindAsync(id);
            if (LeaveAppliction != null)
            {
                _context.LeaveApplictions.Remove(LeaveAppliction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveApplictionExists(int id)
        {
            return _context.LeaveApplictions.Any(e => e.Id == id);
        }
    }
}
