using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.Models
{
    public class HRM_ATD_Shift
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShiftCode { get; set; }
        public string? ShiftName { get; set; }
        public string? ShiftShortName { get; set; }
        public DateTime? ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public DateTime? LateTime { get; set; }
        public DateTime? AbsentTime { get; set; }
        public DateTime? WEF { get; set; }
        public string? Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
