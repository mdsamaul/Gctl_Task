using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.Models
{
    public class HRM_Employee
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AI_ID { get; set; }
        public string? EmployeeID { get; set; }
        public string? Name { get; set; } 
        public string? DesignationCode { get; set; }
    }
}
