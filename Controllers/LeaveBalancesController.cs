using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    public class LeaveBalancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LeaveBalancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            var result = await _context.Employees
                .Include(x => x.Status)
                .ToListAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult AdjustLeaveBalance(int id)
        {
            LeaveAdjustmentEntry leaveAdjustment = new();
            leaveAdjustment.EmployeeId = id;

            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails
                .Include(y => y.SystemCode)
                .Where(x => x.SystemCode.Code=="LeaveAdjustment"), "Id", "Description");
            
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", id);
            ViewData["LeavePeriodId"] = new SelectList(_context.LeavePeriods.Where(x => x.Closed == false), "Id", "Name");
            return View(leaveAdjustment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustLeaveBalance(int id, LeaveAdjustmentEntry leaveAdjustmentEntry)
        {
                
                var adjustmentType = _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveAdjustment" && y.Id == leaveAdjustmentEntry.AdjustmentTypeId)
                .FirstOrDefault();

                leaveAdjustmentEntry.AdjustmentDescription = leaveAdjustmentEntry.AdjustmentDescription +"-"+ adjustmentType.Description;
                leaveAdjustmentEntry.Id = 0;

                var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                leaveAdjustmentEntry.CreatedById = Userid;
                leaveAdjustmentEntry.ModifiedById = "";

                _context.Add(leaveAdjustmentEntry);
                await _context.SaveChangesAsync(Userid);

                var employee = await _context.Employees.FindAsync(leaveAdjustmentEntry.EmployeeId);
                if(adjustmentType.Code == "Positive")
                {
                    employee.LeaveOutstandingBalance = (employee.AllocatedLeaveDays + leaveAdjustmentEntry.NoOfDays);
                }
                else
                {
                    employee.LeaveOutstandingBalance = (employee.AllocatedLeaveDays - leaveAdjustmentEntry.NoOfDays);
                }
                _context.Update(employee);
                await _context.SaveChangesAsync(Userid);

            return RedirectToAction(nameof(Index));
            ViewData["LeavePeriodId"] = new SelectList(_context.LeavePeriods.Where(x => x.Closed == false), "Id", "Name", leaveAdjustmentEntry.LeavePeriodId);
            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
            return View(leaveAdjustmentEntry);
        }
    }
}
