using Microsoft.AspNetCore.Mvc;
using GtclTaskProject.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GtclTaskProject.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();

            var employees = _db.HrmEmployees
                                .Join(_db.HrmDefDesignations,
                                      employee => employee.DesignationCode,
                                      designation => designation.DesignationCode,
                                      (employee, designation) => new
                                      {
                                          employee.AiId,
                                          employee.EmployeeId,
                                          employee.Name,
                                          employee.DesignationCode,
                                          DesignationName = designation.DesignationName
                                      })
                                .ToList();

            return View(employees);
        }


        public IActionResult Delete(int? id)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var em = _db.HrmEmployees.Find(id);
            _db.HrmEmployees.Remove(em);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(List<int> selectedIds)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            if (selectedIds != null && selectedIds.Count > 0)
            {
                foreach (var item in selectedIds)
                {
                    var e = _db.HrmEmployees.Find(item);
                    _db.HrmEmployees.Remove(e);
                    _db.SaveChanges();
                }

            }

            return RedirectToAction("Index");
        }


        //public async Task<IActionResult> EmployeeShift(string searchString, int page = 1, int pageSize = 5)
        //{
        //    GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
        //    int skip = (page - 1) * pageSize;
        //    var rosterEntriesQuery = _db.HrmAtdRosterScheduleEntries.AsQueryable();

        //    // Apply search filter
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        bool isDate = DateTime.TryParseExact(searchString, "yyyy-MM-dd",
        //                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchDate);

        //        bool isInt = int.TryParse(searchString, out int searchInt);

        //        rosterEntriesQuery = rosterEntriesQuery.Where(e =>
        //            (e.EmployeeId != null && e.EmployeeId.Contains(searchString)) ||
        //            (e.RosterScheduleCode != null && e.RosterScheduleCode.Contains(searchString)) ||
        //            (isInt && e.ShiftCode == searchInt) ||
        //            (e.Remarks != null && e.Remarks.Contains(searchString)) ||
        //            (isDate && e.Date != null && e.Date.Date == searchDate.Date));
        //    }

        //    // Get Total number of items from database
        //    var totalCount = await rosterEntriesQuery.CountAsync();

        //    // Get Paginated Data
        //    var rosterEntries = await rosterEntriesQuery
        //        .Skip(skip)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var employees = await _db.HrmEmployees
        //        .Select(emp => new HrmEmployeeViewModel
        //        {
        //            AI_ID = emp.AiId,
        //            EmployeeID = emp.EmployeeId,
        //            Name = emp.Name,
        //            DesignationCode = emp.DesignationCode
        //        })
        //        .ToListAsync();

        //    // Create the ViewModel and assign the data
        //    var viewModel = new EmployeeShiftViewModel
        //    {
        //        Employees = employees,
        //        HrmAtdRosterScheduleEntry = rosterEntries
        //    };

        //    Console.WriteLine($"Current Page: {page}, Page Size: {pageSize}, Total Count: {totalCount}");

        //    ViewBag.SearchString = searchString;
        //    ViewBag.CurrentPage = page;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.TotalCount = totalCount;

        //    return View(viewModel);
        //}


        // Original method for full page load
        public async Task<IActionResult> EmployeeShift(string searchString, int page = 1, int pageSize = 5)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            int skip = (page - 1) * pageSize;
            var rosterEntriesQuery = _db.HrmAtdRosterScheduleEntries.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isDate = DateTime.TryParseExact(searchString, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchDate);
                bool isInt = int.TryParse(searchString, out int searchInt);
                rosterEntriesQuery = rosterEntriesQuery.Where(e =>
                    (e.EmployeeId != null && e.EmployeeId.Contains(searchString)) ||
                    (e.RosterScheduleCode != null && e.RosterScheduleCode.Contains(searchString)) ||
                    (isInt && e.ShiftCode == searchInt) ||
                    (e.Remarks != null && e.Remarks.Contains(searchString)) ||
                    (isDate && e.Date != null && e.Date.Date == searchDate.Date));
            }

            // Get Total number of items from database
            var totalCount = await rosterEntriesQuery.CountAsync();

            // Get Paginated Data
            var rosterEntries = await rosterEntriesQuery
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var employees = await _db.HrmEmployees
                .Select(emp => new HrmEmployeeViewModel
                {
                    AI_ID = emp.AiId,
                    EmployeeID = emp.EmployeeId,
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

        // New method for AJAX table refresh
        public async Task<IActionResult> GetShiftTableData(string searchString, int page = 1, int pageSize = 5)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            int skip = (page - 1) * pageSize;
            var rosterEntriesQuery = _db.HrmAtdRosterScheduleEntries.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isDate = DateTime.TryParseExact(searchString, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchDate);
                bool isInt = int.TryParse(searchString, out int searchInt);
                rosterEntriesQuery = rosterEntriesQuery.Where(e =>
                    (e.EmployeeId != null && e.EmployeeId.Contains(searchString)) ||
                    (e.RosterScheduleCode != null && e.RosterScheduleCode.Contains(searchString)) ||
                    (isInt && e.ShiftCode == searchInt) ||
                    (e.Remarks != null && e.Remarks.Contains(searchString)) ||
                    (isDate && e.Date != null && e.Date.Date == searchDate.Date));
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
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            if (data != null && data.selectedEmployeeIds != null && data.selectedEmployeeIds.Count > 0)
            {
                // Convert fromDate and toDate to DateTime
                if (!DateTime.TryParse(data.fromDate, out DateTime fromDate) ||
                    !DateTime.TryParse(data.toDate, out DateTime toDate))
                {
                    ModelState.AddModelError("", "Invalid date format.");
                    return BadRequest(ModelState);
                }

                // Ensure fromDate is before toDate
                if (fromDate > toDate)
                {
                    ModelState.AddModelError("", "From Date must be before To Date.");
                    return BadRequest(ModelState);
                }

                // Check if dates are within SQL Server's valid range
                DateTime minSqlDate = new DateTime(1753, 1, 1);
                DateTime maxSqlDate = new DateTime(9999, 12, 31);

                if (fromDate < minSqlDate || toDate > maxSqlDate)
                {
                    ModelState.AddModelError("", "Dates must be between January 1, 1753 and December 31, 9999.");
                    return BadRequest(ModelState);
                }

                // Calculate the number of days between fromDate and toDate
                int numberOfDays = (toDate - fromDate).Days + 1;

                try
                {
                    foreach (var employeeId in data.selectedEmployeeIds)
                    {
                        for (int i = 0; i < numberOfDays; i++)
                        {
                            DateTime shiftDate = fromDate.AddDays(i);
                            var employeeShift = new HrmAtdRosterScheduleEntry
                            {
                                RosterScheduleCode = "RS-2024-07-26-001",
                                //EmployeeId = employeeId.ToString(),
                                EmployeeId = employeeId.ToString("D8"),
                                Date = shiftDate,
                                ShiftCode = 1,
                                Remarks = data.shift,
                                EntryDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            // Add the new shift to the database
                            _db.HrmAtdRosterScheduleEntries.Add(employeeShift);
                        }
                    }

                    // Save changes to the database
                    _db.SaveChanges();

                    return RedirectToAction("EmployeeShift");
                }
                catch (Exception ex)
                {
                    // Log the exception details
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
                // If no employees were selected, display an error message
                ModelState.AddModelError("", "Please select at least one employee.");
                return RedirectToAction("EmployeeShift");
            }
        }
        [HttpGet]
        public IActionResult GetRosterData()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            try
            {
                var rosterData = _db.HrmAtdRosterScheduleEntries
                    .OrderByDescending(r => r.AiId)
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