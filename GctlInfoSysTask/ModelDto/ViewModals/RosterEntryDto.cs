using Microsoft.EntityFrameworkCore;

namespace GctlInfoSysTask.ModelDto.ViewModals
{   
        public class RosterEntryDto
        {
            public decimal AiId { get; set; }
            public string EmployeeId { get; set; }
            public string Name { get; set; }
            public string DesignationName { get; set; }
            public DateTime FromDate { get; set; }
            public string ShiftName { get; set; }
            public DateTime TimeFrom { get; set; }
            public DateTime TimeTo { get; set; }
        }
}
//using System.ComponentModel.DataAnnotations;
 
//namespace GCTL_Written_Nazmul.Models.ViewModels
//{
//    public class RosterScheduleViewModel
//    {
//        [Required(ErrorMessage = "From Date is required")]
//        public DateTime FromDate { get; set; }

//        [Required(ErrorMessage = "To Date is required")]
//        public DateTime ToDate { get; set; }

//        [Required(ErrorMessage = "Shift is required")]
//        public int ShiftCode { get; set; }

//        [Required(ErrorMessage = "Please select at least one employee")]
//        public List<string> SelectedEmployees { get; set; }

//        public string? Remarks { get; set; }
//    }
//}

//namespace GCTL_Written_Nazmul.Models.ViewModels
//{
//    [Keyless]
//    public class RosterView
//    {
//        public decimal AiId { get; set; }
//        public string EmployeeId { get; set; }
//        public string Name { get; set; }
//        public string DesignationName { get; set; }
//        public DateTime FromDate { get; set; }
//        public string ShiftName { get; set; }
//        public DateTime TimeFrom { get; set; }
//        public DateTime TimeTo { get; set; }
//    }
//}

//namespace GCTL_Written_Nazmul.Models.ViewModels
//{
//    public class UpdateRosterEntryViewModel
//    {
//        public decimal AiId { get; set; }
//        public DateTime Date { get; set; }
//        public int ShiftCode { get; set; }
//        public string Remarks { get; set; }
//    }
//}
