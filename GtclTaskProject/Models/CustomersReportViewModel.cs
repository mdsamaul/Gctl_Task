namespace GtclTaskProject.Models
{
    public class CustomersReportViewModel
    {
        public int SlNo { get; set; }  
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public CustomerType? CustomerType { get; set; }
        public DateTime? BusinessStart { get; set; } 
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal? CreditLimit { get; set; } 
        public string Photo { get; set; } 
        public string DeliveryAddressInfo { get; set; }
    }
}
