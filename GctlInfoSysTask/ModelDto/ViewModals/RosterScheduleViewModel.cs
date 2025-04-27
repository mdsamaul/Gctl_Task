using System.ComponentModel.DataAnnotations;

namespace GctlInfoSysTask.ModelDto.ViewModals
{
    public class RosterScheduleViewModel
    {
        [Required(ErrorMessage = "From Date is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To Date is required")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Shift is required")]
        public int ShiftCode { get; set; }

        [Required(ErrorMessage = "Please select at least one employee")]
        public List<string> SelectedEmployees { get; set; }

        public string? Remarks { get; set; }
        public int AI_ID { get; set; }
        public string RosterScheduleCode { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } // Optional: If you want to join with employee table
        public DateTime Date { get; set; }
        public string ShiftName { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
