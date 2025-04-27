using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GctlInfoSysTask.Models
{
    public class DeliveryAddress
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryAddressId { get; set; }

        public string? DeliveryAddressLine { get; set; }
        public string? ContactPerson { get; set; }    
        public string? Phone { get; set; }
        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
    }

}
