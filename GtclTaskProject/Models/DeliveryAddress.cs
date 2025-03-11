using System;
using System.Collections.Generic;

namespace GtclTaskProject.Models;

public partial class DeliveryAddress
{
    public int DeliveryAddressId { get; set; }

    public string? DeliveryAddressLine { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
