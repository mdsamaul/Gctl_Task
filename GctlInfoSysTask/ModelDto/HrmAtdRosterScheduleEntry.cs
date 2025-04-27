using GctlInfoSysTask.Models;

namespace GctlInfoSysTask.ModelDto
{
    public class HrmAtdRosterScheduleEntry
    {
        public int? AI_ID { get; set; }
        public string RosterScheduleCode { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ShiftCode { get; set; }
        public string? Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        // Property to hold selected employee IDs
        public List<int> SelectedEmployeeId { get; set; } = new List<int>();

        // Navigation property for employees list
        public List<HRM_Employee> hRM_Employees { get; set; } = new List<HRM_Employee>();
    }
}
