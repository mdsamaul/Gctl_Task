using GctlInfoSysTask.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GctlInfoSysTask.ModelDto
{
    public class CustomerDto
    {
        public int AI_ID { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        [Display(Name = "Business Start")]
        public DateTime? BusinessStart { get; set; }
        [Display(Name = "Customer Type")]
        public int? CustomerTypeId { get; set; }
        public virtual CustomerType? CustomerType { get; set; }
        [Phone(ErrorMessage = "Pleace Enter valid Phone Number")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Pleace Enter valid Phone Number")]
        public string? Phone { get; set; }
        [EmailAddress(ErrorMessage = "Pleace Enter  valid Email")]
        public string? Email { get; set; }
        [Display(Name = "Credit Limit")]
        public int? CreditLimit { get; set; }
        public string? Photo { get; set; }
        [Display(Name = "Photo")]
        public IFormFile? PhotoFile { get; set; }
        public string? DeliveryAddressLine { get; set; }
        public string? ContactPerson { get; set; }
        public string? DeliveryPhone { get; set; }
        public virtual ICollection<DeliveryAddressDto>? DeliveryAddressDtos { get; set; }
        public ICollection<DeliveryAddress>? DeliveryAddresses { get; set; }
    }
}
