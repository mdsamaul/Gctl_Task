using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.Models
{
    public class HRM_ATD_RosterScheduleEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AI_ID { get; set; }
        public string? RosterScheduleCode { get; set; } 
        public string? EmployeeID { get; set; }
        public DateTime? Date { get; set; }
        public int? ShiftCode { get; set; }
        public string? Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }

}
