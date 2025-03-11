namespace GtclTaskProject.Models
{
    public class EmployeeShiftViewModal
    {
        public int ID { get; set; }
        public string EmpID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Shift { get; set; }
        public TimeSpan TimeFrom { get; set; } 
        public TimeSpan TimeTo { get; set; }   
        public string Action { get; set; }
    }
}
