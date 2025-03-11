namespace GtclTaskProject.Models
{
    public class ShiftAssignmentData
    {
        public List<int> selectedEmployeeIds { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string shift { get; set; }
    }
}
