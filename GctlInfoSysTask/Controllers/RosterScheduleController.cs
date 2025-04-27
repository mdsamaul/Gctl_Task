using GctlInfoSysTask.Data;
using GctlInfoSysTask.Migrations;
using GctlInfoSysTask.ModelDto;
using GctlInfoSysTask.ModelDto.ViewModals;
using GctlInfoSysTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Globalization;

namespace GctlInfoSysTask.Controllers
{
    public class RosterScheduleController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public RosterScheduleController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            var employeeList = _context.Set<HRM_Employee>().FromSqlRaw("SELECT * FROM vw_EmployeeList").ToList();
            return View(employeeList);
        }
        public IActionResult RosterEntityForm()
        {
            return View();
        }
        public IActionResult RosterEntityData()
        {
            return View();
        }
        public IActionResult EmployeeSelectView()
        {
            var employeeList = _context.Set<HRM_Employee>().FromSqlRaw("SELECT * FROM vw_EmployeeList").ToList();
            return View(employeeList);
        }


        [HttpPost]
        public JsonResult LoadEmployee()
        {
            try
            {
                // Get pagination parameters
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 10;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Get data from your SQL view
                var query = _context.HRM_Employee.AsQueryable();

                // Get total count
                int totalRecords = query.Count();

                // Apply filtering
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(x =>
                        x.EmployeeID.Contains(searchValue) ||
                        x.Name.Contains(searchValue) ||
                        x.DesignationCode.Contains(searchValue));
                }

                int filteredRecords = query.Count();

                // Apply pagination and ordering
                var data = query
                    .OrderByDescending(x => x.EmployeeID)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        AI_ID = x.AI_ID,
                        EmployeeID = x.EmployeeID,
                        Name = x.Name,
                        DesignationCode = x.DesignationCode
                    })
                    .ToList();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
      

        public IActionResult AssignShift()
        {
            var model = new HrmAtdRosterScheduleEntry
            {
                //hRM_Employees = _context.HRM_Employee.ToList()
                hRM_Employees = _context.Set<HRM_Employee>().FromSqlRaw("SELECT * FROM vw_EmployeeList").ToList()
            };
            return View(model);
        }

        //[HttpPost]
        //public IActionResult AssignShift(HrmAtdRosterScheduleEntry model)
        //{


        //    // Validate input
        //    if (model.SelectedEmployeeId == null || !model.SelectedEmployeeId.Any())
        //    {
        //        ViewBag.ErrorMessage = "Please select at least one employee.";
        //        model.hRM_Employees = _context.HRM_Employee.ToList();
        //        return View(model);
        //    }

        //    if (!model.FromDate.HasValue || !model.ToDate.HasValue)
        //    {
        //        ViewBag.ErrorMessage = "Please select both From and To dates.";
        //        model.hRM_Employees = _context.HRM_Employee.ToList();
        //        return View(model);
        //    }

        //    if (!model.ShiftCode.HasValue)
        //    {
        //        ViewBag.ErrorMessage = "Please select a shift.";
        //        model.hRM_Employees = _context.HRM_Employee.ToList();
        //        return View(model);
        //    }

        //    try
        //    {
        //        // Generate a unique roster schedule code
        //        string rosterScheduleCode = "RS" + DateTime.Now.ToString("yyyyMMddHHmmss");

        //        // Create DataTable for bulk insert
        //        DataTable dataTable = new DataTable();
        //        dataTable.Columns.Add("RosterScheduleCode", typeof(string));
        //        dataTable.Columns.Add("EmployeeID", typeof(int));
        //        dataTable.Columns.Add("Date", typeof(DateTime));
        //        dataTable.Columns.Add("ShiftCode", typeof(int));
        //        dataTable.Columns.Add("EntryDate", typeof(DateTime));
        //        dataTable.Columns.Add("Remarks", typeof(string));

        //        // Fill DataTable with data for each selected employee and each day in the date range
        //        foreach (var employeeId in model.SelectedEmployeeId)
        //        {
        //            DateTime fromDate = model.FromDate.Value;
        //            DateTime toDate = model.ToDate.Value;

        //            for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
        //            {
        //                dataTable.Rows.Add(
        //                    rosterScheduleCode,
        //                    employeeId,
        //                    date,
        //                    model.ShiftCode.Value,
        //                    DateTime.Now,
        //                    null // Remarks (can be null)
        //                );
        //            }
        //        }

        //        // Get the connection string from configuration
        //        string connectionString = _configuration.GetConnectionString("AppCon");

        //        // Fallback to getting it from the context if needed
        //        if (string.IsNullOrEmpty(connectionString))
        //        {
        //            connectionString = _context.Database.GetDbConnection().ConnectionString;
        //        }

        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
        //            {
        //                bulkCopy.DestinationTableName = "HRM_ATD_RosterScheduleEntry";

        //                // Map columns from DataTable to database table
        //                bulkCopy.ColumnMappings.Add("RosterScheduleCode", "RosterScheduleCode");
        //                bulkCopy.ColumnMappings.Add("EmployeeID", "EmployeeID");
        //                bulkCopy.ColumnMappings.Add("Date", "Date");
        //                bulkCopy.ColumnMappings.Add("ShiftCode", "ShiftCode");
        //                bulkCopy.ColumnMappings.Add("EntryDate", "EntryDate");
        //                bulkCopy.ColumnMappings.Add("Remarks", "Remarks");

        //                // Execute bulk insert
        //                bulkCopy.WriteToServer(dataTable);
        //            }
        //        }

        //        // Success message
        //        TempData["SuccessMessage"] = $"Successfully assigned shifts to {model.SelectedEmployeeId.Count} employees for the selected date range.";
        //        return RedirectToAction("AssignShift");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = $"Error occurred: {ex.Message}";
        //        model.hRM_Employees = _context.HRM_Employee.ToList();
        //        return View(model);
        //    }
        //}



        [HttpPost]
        public IActionResult AssignShift(HrmAtdRosterScheduleEntry model)
        {
            // Validation checks
            if (model.SelectedEmployeeId == null || !model.SelectedEmployeeId.Any())
            {
                ViewBag.ErrorMessage = "Please select at least one employee.";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }

            if (!model.FromDate.HasValue || !model.ToDate.HasValue)
            {
                ViewBag.ErrorMessage = "Please select both From and To dates.";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }

            if (!model.ShiftCode.HasValue)
            {
                ViewBag.ErrorMessage = "Please select a shift.";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }

            try
            {
                // Step 1: Delete existing roster entries for selected employees within date range
                var selectedIds = model.SelectedEmployeeId;
                var fromDate = model.FromDate.Value;
                var toDate = model.ToDate.Value;

                string deleteQuery = $@"
            DELETE FROM HRM_ATD_RosterScheduleEntry
            WHERE EmployeeID IN ({string.Join(",", selectedIds)})
            AND [Date] BETWEEN @fromDate AND @toDate
        ";

                _context.Database.ExecuteSqlRaw(deleteQuery,
                    new SqlParameter("@fromDate", fromDate),
                    new SqlParameter("@toDate", toDate));

                // 🟢 Step 2: Generate unique roster schedule code
                string rosterScheduleCode = "RS" + DateTime.Now.ToString("yyyyMMddHHmmss");

                // Step 3: Create and fill DataTable for bulk insert
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("RosterScheduleCode", typeof(string));
                //dataTable.Columns.Add("EmployeeID", typeof(int));
                dataTable.Columns.Add("EmployeeID", typeof(string));
                dataTable.Columns.Add("Date", typeof(DateTime));
                dataTable.Columns.Add("ShiftCode", typeof(int));
                dataTable.Columns.Add("EntryDate", typeof(DateTime));
                dataTable.Columns.Add("Remarks", typeof(string));

                foreach (var employeeId in model.SelectedEmployeeId)
                {
                    for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
                    {
                        dataTable.Rows.Add(
                            rosterScheduleCode,
                            employeeId.ToString("D8"),
                            date,
                            model.ShiftCode.Value,
                            DateTime.Now,
                            model.Remarks
                        );
                    }
                }

                // Step 4: Get connection string
                string connectionString = _configuration.GetConnectionString("AppCon");
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = _context.Database.GetDbConnection().ConnectionString;
                }

                // Step 5: Perform bulk insert
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "HRM_ATD_RosterScheduleEntry";

                        bulkCopy.ColumnMappings.Add("RosterScheduleCode", "RosterScheduleCode");
                        bulkCopy.ColumnMappings.Add("EmployeeID", "EmployeeID");
                        bulkCopy.ColumnMappings.Add("Date", "Date");
                        bulkCopy.ColumnMappings.Add("ShiftCode", "ShiftCode");
                        bulkCopy.ColumnMappings.Add("EntryDate", "EntryDate");
                        bulkCopy.ColumnMappings.Add("Remarks", "Remarks");

                        bulkCopy.WriteToServer(dataTable);
                    }
                }

                TempData["SuccessMessage"] = $"Successfully assigned shifts to {model.SelectedEmployeeId.Count} employees from {fromDate.ToShortDateString()} to {toDate.ToShortDateString()}.";

                return RedirectToAction("AssignShift");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error occurred: {ex.Message}";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }
        }






        //public IActionResult ViewRosterSchedules()
        //{
        //    // Option 1: Using raw SQL to query the view
        //    var rosterSchedules = _context.Set<RosterScheduleViewModel>()
        //        .FromSqlRaw("SELECT * FROM vw_HRM_ATD_RosterScheduleEntry")
        //        .ToList();

        //    return View(rosterSchedules);
        //}


        //public IActionResult ViewRosterSchedules()
        //{
        //    var rosterSchedules = new List<RosterScheduleViewModel>();

        //    string connectionString = _configuration.GetConnectionString("AppCon");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand("SELECT * FROM vw_HRM_ATD_RosterScheduleEntry", connection))
        //        {
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    rosterSchedules.Add(new RosterScheduleViewModel
        //                    {
        //                        AI_ID = reader.IsDBNull(reader.GetOrdinal("AI_ID")) ? 0 : (reader["AI_ID"] is DBNull ? 0 : Convert.ToInt32(reader["AI_ID"])),
        //                        RosterScheduleCode = reader.GetString(reader.GetOrdinal("RosterScheduleCode")),
        //                        EmployeeID = reader.IsDBNull(reader.GetOrdinal("EmployeeID")) ? 0 : (reader["EmployeeID"] is DBNull ? 0 : Convert.ToInt32(reader["EmployeeID"])),
        //                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
        //                        ShiftCode = reader.IsDBNull(reader.GetOrdinal("ShiftCode")) ? 0 : (reader["ShiftCode"] is DBNull ? 0 : Convert.ToInt32(reader["ShiftCode"])),
        //                        ShiftName = reader.IsDBNull(reader.GetOrdinal("ShiftName")) ? null : reader.GetString(reader.GetOrdinal("ShiftName")),
        //                        Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
        //                        EntryDate = reader.GetDateTime(reader.GetOrdinal("EntryDate")),
        //                        ModifyDate = reader.IsDBNull(reader.GetOrdinal("ModifyDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifyDate"))
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return View(rosterSchedules);
        //}


        public IActionResult RosterEntityView()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult GetData()
        //{
        //    try
        //    {
        //        var draw = Request.Form["draw"].FirstOrDefault();
        //        var start = Request.Form["start"].FirstOrDefault();
        //        var length = Request.Form["length"].FirstOrDefault();
        //        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        //        int pageSize = length != null ? Convert.ToInt32(length) : 10;
        //        int skip = start != null ? Convert.ToInt32(start) : 0;


        //        var query = _context.Set<HRM_ATD_RosterScheduleEntry>()
        //            .FromSqlRaw("SELECT * FROM vw_HRM_ATD_RosterScheduleEntry");


        //        if (!string.IsNullOrEmpty(searchValue))
        //        {
        //            query = query.Where(x =>
        //                x.AI_ID.ToString().Contains(searchValue) ||
        //                x.EmployeeID.ToString().Contains(searchValue) ||
        //                x.Date.ToString().Contains(searchValue) ||
        //                x.RosterScheduleCode.Contains(searchValue) ||
        //                x.Remarks.Contains(searchValue)
        //            );
        //        }

        //        var totalRecords = query.Count();
        //        var filteredRecords = query.Count();

        //        var data = query.OrderByDescending(x => x.AI_ID)
        //                        .Skip(skip)
        //                        .Take(pageSize)
        //                        .ToList()
        //                        .Select(x => new
        //                        {
        //                            x.AI_ID,
        //                            x.RosterScheduleCode,
        //                            x.EmployeeID,
        //                            x.Date,
        //                            x.ShiftCode,
        //                            x.Remarks,
        //                            x.EntryDate,
        //                            x.ModifyDate
        //                        });

        //        return Json(new
        //        {
        //            draw = draw,
        //            recordsTotal = totalRecords,
        //            recordsFiltered = filteredRecords,
        //            data = data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}


        /////fiexd  get data 
        //[HttpPost]
        //public IActionResult GetData()
        //{
        //    try
        //    {
        //        var draw = Request.Form["draw"].FirstOrDefault();
        //        var start = Request.Form["start"].FirstOrDefault();
        //        var length = Request.Form["length"].FirstOrDefault();
        //        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        //        int pageSize = length != null ? Convert.ToInt32(length) : 10;
        //        int skip = start != null ? Convert.ToInt32(start) : 0;                

        //        var query = (from rs in _context.Set<HRM_ATD_RosterScheduleEntry>()
        //                     join e in _context.Set<HRM_Employee>()
        //                         on rs.EmployeeID equals e.EmployeeID
        //                     join s in _context.Set<HRM_ATD_Shift>()
        //                         on rs.ShiftCode equals s.ShiftCode
        //                     join d in _context.Set<HRM_Def_Designation>()
        //                         on e.DesignationCode equals d.DesignationCode
        //                     select new
        //                     {
        //                         rs.AI_ID,
        //                         rs.RosterScheduleCode,
        //                         rs.EmployeeID,
        //                         EmployeeName = e.Name,
        //                         rs.Date,
        //                         DayName = rs.Date.HasValue ? rs.Date.Value.DayOfWeek.ToString() : "Unknown",
        //                         rs.ShiftCode,
        //                         DesignationName = d.DesignationName,
        //                         ShiftName = s.ShiftName ?? (rs.ShiftCode == 1 ? "Morning"
        //                                             : rs.ShiftCode == 2 ? "Evening"
        //                                             : rs.ShiftCode == 3 ? "Night"
        //                                             : "Unknown"),
        //                         rs.Remarks,
        //                         rs.EntryDate,
        //                         rs.ModifyDate
        //                     }).AsEnumerable(); 

        //        if (!string.IsNullOrEmpty(searchValue))
        //        {
        //            DateTime searchDate;
        //            bool isExactDateSearch = DateTime.TryParseExact(searchValue, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out searchDate);

        //            bool isMonthSearch = false;
        //            int month = 0;
        //            if (searchValue.Contains("/") && !isExactDateSearch)
        //            {
        //                string[] parts = searchValue.Split('/');
        //                if (parts.Length >= 1)
        //                {
        //                    int.TryParse(parts[0], out month);
        //                    isMonthSearch = month >= 1 && month <= 12;
        //                }
        //            }

        //            query = query.Where(x =>
        //                x.AI_ID.ToString().Contains(searchValue) ||
        //                x.EmployeeID.ToString().Contains(searchValue) ||
        //                (isExactDateSearch && x.Date.HasValue && x.Date.Value.Date == searchDate.Date) ||
        //                (isMonthSearch && x.Date.HasValue && x.Date.Value.Month == month) ||
        //                (!string.IsNullOrEmpty(x.RosterScheduleCode) && x.RosterScheduleCode.Contains(searchValue)) ||
        //                (!string.IsNullOrEmpty(x.Remarks) && x.Remarks.Contains(searchValue)) ||
        //                (!string.IsNullOrEmpty(x.EmployeeName) && x.EmployeeName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
        //            );
        //        }


        //        var totalRecords = query.Count(); // Total records before filtering
        //        var filteredRecords = query.Count(); // Same count until filtered

        //        if (!string.IsNullOrEmpty(searchValue))
        //        {
        //            filteredRecords = query.Count(); // Adjust filtered records if search value is provided
        //        }

        //        var data = query.OrderByDescending(x => x.AI_ID)
        //                        .Skip(skip)
        //                        .Take(pageSize)
        //                        .ToList();

        //        return Json(new
        //        {
        //            draw = draw,
        //            recordsTotal = totalRecords,
        //            recordsFiltered = filteredRecords,
        //            data = data
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 10;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // প্রথমে Raw Query করি
                var query = from rs in _context.Set<HRM_ATD_RosterScheduleEntry>()
                            join e in _context.Set<HRM_Employee>()
                                on rs.EmployeeID equals e.EmployeeID
                            join s in _context.Set<HRM_ATD_Shift>()
                                on rs.ShiftCode equals s.ShiftCode
                            join d in _context.Set<HRM_Def_Designation>()
                                on e.DesignationCode equals d.DesignationCode
                            select new
                            {
                                rs.AI_ID,
                                rs.RosterScheduleCode,
                                rs.EmployeeID,
                                EmployeeName = e.Name,
                                rs.Date,
                                rs.ShiftCode,
                                DesignationName = d.DesignationName,
                                ShiftName = s.ShiftName,
                                rs.Remarks,
                                rs.EntryDate,
                                rs.ModifyDate
                            };

                var rawData = await query.ToListAsync(); 

                var dataEnumerable = rawData.Select(x => new
                {
                    x.AI_ID,
                    x.RosterScheduleCode,
                    x.EmployeeID,
                    x.EmployeeName,
                    x.Date,
                    DayName = x.Date.HasValue ? x.Date.Value.DayOfWeek.ToString() : "Unknown",
                    x.ShiftCode,
                    x.DesignationName,
                    ShiftName = !string.IsNullOrEmpty(x.ShiftName) ? x.ShiftName :
                                (x.ShiftCode == 1 ? "Morning" :
                                 x.ShiftCode == 2 ? "Evening" :
                                 x.ShiftCode == 3 ? "Night" : "Unknown"),
                    x.Remarks,
                    x.EntryDate,
                    x.ModifyDate
                });

                if (!string.IsNullOrEmpty(searchValue))
                {
                    DateTime searchDate;
                    bool isExactDateSearch = DateTime.TryParseExact(searchValue, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out searchDate);

                    bool isMonthSearch = false;
                    int month = 0;
                    if (searchValue.Contains("/") && !isExactDateSearch)
                    {
                        string[] parts = searchValue.Split('/');
                        if (parts.Length >= 1)
                        {
                            int.TryParse(parts[0], out month);
                            isMonthSearch = month >= 1 && month <= 12;
                        }
                    }

                    dataEnumerable = dataEnumerable.Where(x =>
                        x.AI_ID.ToString().Contains(searchValue) ||
                        x.EmployeeID.ToString().Contains(searchValue) ||
                        (isExactDateSearch && x.Date.HasValue && x.Date.Value.Date == searchDate.Date) ||
                        (isMonthSearch && x.Date.HasValue && x.Date.Value.Month == month) ||
                        (!string.IsNullOrEmpty(x.RosterScheduleCode) && x.RosterScheduleCode.Contains(searchValue)) ||
                        (!string.IsNullOrEmpty(x.Remarks) && x.Remarks.Contains(searchValue)) ||
                        (!string.IsNullOrEmpty(x.EmployeeName) && x.EmployeeName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    );
                }

                var totalRecords = rawData.Count;
                var filteredRecords = dataEnumerable.Count();

                var data = dataEnumerable.OrderByDescending(x => x.AI_ID)
                                         .Skip(skip)
                                         .Take(pageSize)
                                         .ToList();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult DeleteShift(HrmAtdRosterScheduleEntry model)
        {
            if (model.SelectedEmployeeId == null || !model.SelectedEmployeeId.Any())
            {
                ViewBag.ErrorMessage = "Please select at least one employee.";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }

            if (!model.FromDate.HasValue || !model.ToDate.HasValue)
            {
                ViewBag.ErrorMessage = "Please select both From and To dates.";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }

            if (!model.ShiftCode.HasValue)
            {
                ViewBag.ErrorMessage = "Please select a shift.";
                model.hRM_Employees = _context.HRM_Employee.ToList();
                return View(model);
            }

            try
            {
                var selectedIds = model.SelectedEmployeeId;
                var fromDate = model.FromDate.Value;
                var toDate = model.ToDate.Value;

                string deleteQuery = $@"
            DELETE FROM HRM_ATD_RosterScheduleEntry
            WHERE EmployeeID IN ({string.Join(",", selectedIds)})
            AND [Date] BETWEEN @fromDate AND @toDate
        ";

                _context.Database.ExecuteSqlRaw(deleteQuery,
                    new SqlParameter("@fromDate", fromDate),
                    new SqlParameter("@toDate", toDate));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            TempData["deleteSuccessMessage"] = "Roster entries deleted successfully!";
            return RedirectToAction("AssignShift");
        }

    }
}
