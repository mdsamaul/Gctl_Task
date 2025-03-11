using System;
using System.Collections.Generic;

namespace GtclTaskProject.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? Address { get; set; }

    public DateTime? BusinessStart { get; set; }

    public CustomerType? CustomerType { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal? CreditLimit { get; set; }

    public string? Photo { get; set; }

    public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();
}
