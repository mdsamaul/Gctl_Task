using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GctlInfoSysTask.Models
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AI_ID { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public DateTime? BusinessStart { get; set; }
        [ForeignKey("CustomerType")]
        public int? CustomerTypeId { get; set; }
        public virtual CustomerType? CustomerType { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? CreditLimit { get; set; }
        public string? Photo { get; set; }

        public virtual ICollection<DeliveryAddress>? DeliveryAddresses { get; set; }
    }

}
