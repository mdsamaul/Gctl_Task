using GctlInfoSysTask.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.ModelDto
{
    public class DeliveryAddressDto
    {
        //public int DeliveryAddressId { get; set; }
        //public string? DeliveryAddressLine { get; set; }
        //public string? ContactPerson { get; set; }
        //public string? Phone { get; set; }
        public int DeliveryAddressId { get; set; }

        public string? DeliveryAddressLine { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
