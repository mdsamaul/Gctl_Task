using Microsoft.EntityFrameworkCore;

namespace GctlInfoSysTask.ModelDto.ViewModals
{
    [Keyless]
    public class RosterView
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
