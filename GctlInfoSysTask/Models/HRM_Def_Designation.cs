using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.Models
{
    public class HRM_Def_Designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AI_ID { get; set; }
        public string? DesignationCode { get; set; }
        public string? DesignationName { get; set; }
        public string? DesignationShortName { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }

}
