using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.Models
{
    public class CustomerType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }
        public virtual ICollection<Customer>? Customers { get; set; }
    }

}
