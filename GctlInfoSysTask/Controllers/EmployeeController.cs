using Microsoft.AspNetCore.Mvc;
using GctlInfoSysTask.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using GctlInfoSysTask.Data;
using GctlInfoSysTask.ModelDto;
using GctlInfoSysTask.Models;
using Microsoft.Extensions.Caching.Memory;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using EFCore.BulkExtensions;

namespace GtclTaskProject.Controllers
{
    public class EmployeeController : Controller
    {
       
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeController> _logger; 

        public EmployeeController(AppDbContext context, ILogger<EmployeeController> logger = null)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {

            //var employees = _context.HRM_Employee
            //                    .Join(_context.HRM_Def_Designation,
            //                          employee => employee.DesignationCode,
            //                          designation => designation.DesignationCode,
            //                          (employee, designation) => new
            //                          {
            //                              employee.AI_ID,
            //                              employee.EmployeeID,
            //                              employee.Name,
            //                              employee.DesignationCode,
            //                              DesignationName = designation.DesignationName
            //                          })
            //                    .ToList();
            //var employees = await _context.HRM_Employee.ToListAsync();
            var employees = await _context.hrmEmployeeViewModels.ToListAsync(); 

            return View(employees);
        }



       

        public IActionResult Delete(int? id)
        {
            var em = _context.HRM_Employee.Find(id);
            _context.HRM_Employee.Remove(em);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(List<int> selectedIds)
        {

            if (selectedIds != null && selectedIds.Count > 0)
            {
                foreach (var item in selectedIds)
                {
                    var e = _context.HRM_Employee.Find(item);
                    _context.HRM_Employee.Remove(e);
                    _context.SaveChanges();
                }

            }

            return RedirectToAction("Index");
        }



        //Original method for full page load
        public async Task<IActionResult> EmployeeShift(string searchString, int page = 1, int pageSize = 5)
        {
            int skip = (page - 1) * pageSize;
            var rosterEntriesQuery = _context.HRM_ATD_RosterScheduleEntry.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isDate = DateTime.TryParseExact(searchString, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchDate);
                bool isInt = int.TryParse(searchString, out int searchInt);
                rosterEntriesQuery = rosterEntriesQuery.Where(e =>
                    (e.EmployeeID != null && e.EmployeeID.Contains(searchString)) ||
                    (e.RosterScheduleCode != null && e.RosterScheduleCode.Contains(searchString)) ||
                    (isInt && e.ShiftCode == searchInt) ||
                    (e.Remarks != null && e.Remarks.Contains(searchString)) ||
                    (isDate && e.Date != null && e.Date == searchDate.Date));
            }

            // Get Total number of items from database
            var totalCount = await rosterEntriesQuery.CountAsync();

            // Get Paginated Data
            var rosterEntries = await rosterEntriesQuery
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            //var employees = await _context.HRM_Employee
            //    .Select(emp => new HrmEmployeeViewModel
            //    {
            //        AI_ID = emp.AI_ID,
            //        EmployeeID = emp.EmployeeID,
            //        Name = emp.Name,
            //        DesignationCode = emp.DesignationCode
            //    })
            //    .ToListAsync();
            var employees = await _context.hrmEmployeeViewModels
                .Select(emp => new HrmEmployeeViewModel
                {
                    AI_ID = emp.AI_ID,
                    EmployeeID = emp.EmployeeID,
                    Name = emp.Name,
                    DesignationCode = emp.DesignationCode
                })
                .ToListAsync();

            // Create the ViewModel and assign the data
            var viewModel = new EmployeeShiftViewModel
            {
                Employees = employees,
                HrmAtdRosterScheduleEntry = rosterEntries
            };

            // Calculate pagination details
            int startRecord = (page - 1) * pageSize + 1;
            int endRecord = Math.Min(startRecord + pageSize - 1, totalCount);
            int displayedCount = rosterEntries.Count;

            ViewBag.SearchString = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.StartRecord = startRecord;
            ViewBag.EndRecord = endRecord;
            ViewBag.DisplayedCount = displayedCount;

            return View(viewModel);
        }

        //New method for AJAX table refresh
        public async Task<IActionResult> GetShiftTableData(string searchString, int page = 1, int pageSize = 5)
        {

            int skip = (page - 1) * pageSize;
            var rosterEntriesQuery = _context.HRM_ATD_RosterScheduleEntry.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isDate = DateTime.TryParseExact(searchString, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchDate);
                bool isInt = int.TryParse(searchString, out int searchInt);
                rosterEntriesQuery = rosterEntriesQuery.Where(e =>
                    (e.EmployeeID != null && e.EmployeeID.Contains(searchString)) ||
                    (e.RosterScheduleCode != null && e.RosterScheduleCode.Contains(searchString)) ||
                    (isInt && e.ShiftCode == searchInt) ||
                    (e.Remarks != null && e.Remarks.Contains(searchString)) ||
                    (isDate && e.Date != null && e.Date == searchDate.Date));
            }

            // Get Total number of items from database
            var totalCount = await rosterEntriesQuery.CountAsync();

            // Get Paginated Data
            var rosterEntries = await rosterEntriesQuery
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Create the ViewModel and assign the data
            var viewModel = new EmployeeShiftViewModel
            {
                HrmAtdRosterScheduleEntry = rosterEntries
            };

            // Calculate pagination details
            int startRecord = (page - 1) * pageSize + 1;
            int endRecord = Math.Min(startRecord + pageSize - 1, totalCount);
            int displayedCount = rosterEntries.Count;

            ViewBag.SearchString = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.StartRecord = startRecord;
            ViewBag.EndRecord = endRecord;
            ViewBag.DisplayedCount = displayedCount;

            return PartialView("_ShiftTablePartial", viewModel);
        }


        [HttpPost]
        public IActionResult AssignShifts([FromBody] ShiftAssignmentData data)
        {
            if (data != null && data.selectedEmployeeIds != null && data.selectedEmployeeIds.Count > 0)
            {
                if (!DateTime.TryParse(data.fromDate, out DateTime fromDate) ||
                    !DateTime.TryParse(data.toDate, out DateTime toDate))
                {
                    ModelState.AddModelError("", "Invalid date format.");
                    return BadRequest(ModelState);
                }

                if (fromDate > toDate)
                {
                    ModelState.AddModelError("", "From Date must be before To Date.");
                    return BadRequest(ModelState);
                }

                DateTime minSqlDate = new DateTime(1753, 1, 1);
                DateTime maxSqlDate = new DateTime(9999, 12, 31);

                if (fromDate < minSqlDate || toDate > maxSqlDate)
                {
                    ModelState.AddModelError("", "Dates must be between January 1, 1753 and December 31, 9999.");
                    return BadRequest(ModelState);
                }

                int numberOfDays = (toDate - fromDate).Days + 1;

                try
                {
                    var shiftEntries = new List<HRM_ATD_RosterScheduleEntry>();

                    foreach (var employeeId in data.selectedEmployeeIds)
                    {
                        for (int i = 0; i < numberOfDays; i++)
                        {
                            DateTime shiftDate = fromDate.AddDays(i);
                            shiftEntries.Add(new HRM_ATD_RosterScheduleEntry
                            {
                                RosterScheduleCode = "RS-2024-07-26-001",
                                EmployeeID = employeeId.ToString("D8"),
                                Date = shiftDate,
                                ShiftCode = 1,
                                Remarks = data.shift,
                                EntryDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            });
                        }
                    }

                    // ✅ Bulk Insert using EFCore.BulkExtensions
                    _context.BulkInsert(shiftEntries);

                    return RedirectToAction("EmployeeShift");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving shifts: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", "Inner error: " + ex.InnerException.Message);
                    }
                    return RedirectToAction("EmployeeShift");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select at least one employee.");
                return RedirectToAction("EmployeeShift");
            }
        }


        [HttpGet]
        public IActionResult GetRosterData()
        {

            try
            {
                var rosterData = _context.HRM_ATD_RosterScheduleEntry
                    .OrderByDescending(r => r.AI_ID)
                    .ToList();

                return Json(rosterData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}