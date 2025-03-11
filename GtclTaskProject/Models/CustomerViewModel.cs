using System.ComponentModel.DataAnnotations;

namespace GtclTaskProject.Models
{
    public class CustomerViewModel
    {
        internal bool RemoveExistingPhoto;

        public CustomerViewModel()
        {
            this.DeliveryAddresses = new List<DeliveryAddressViewModel>();
        }
        public int? CustomerId { get; set; }

       public string? CustomerName { get; set; }

        public string? Address { get; set; }

        public DateTime? BusinessStart { get; set; }

        [Display(Name = "Customer Type")] 
        public CustomerType? CustomerType { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")] 
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Credit is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Credit Limit must be a non-negative number")]
        [Display(Name = "Credit Limit")]
        public decimal? CreditLimit { get; set; }

        public string? Photo { get; set; }

        public IFormFile? PhotoFile { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? ContactPerson { get; set; }
        public string? DeliveryPhone { get; set; }

        //public virtual List<DeliveryAddress>? DeliveryAddresses { get; set; }
        public List<DeliveryAddressViewModel> DeliveryAddresses { get; set; }
                
    }
    
}
