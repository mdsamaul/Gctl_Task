using System;
using System.Collections.Generic;

namespace GtclTaskProject.Models;

public partial class HrmEmployee
{
    public int AiId { get; set; }

    public string EmployeeId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string DesignationCode { get; set; } = null!;
}
