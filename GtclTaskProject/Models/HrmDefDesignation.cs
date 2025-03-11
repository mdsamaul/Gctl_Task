using System;
using System.Collections.Generic;

namespace GtclTaskProject.Models;

public partial class HrmDefDesignation
{
    public int AiId { get; set; }

    public string DesignationCode { get; set; } = null!;

    public string? DesignationName { get; set; }

    public string? DesignationShortName { get; set; }

    public DateTime? EntryDate { get; set; }

    public DateTime? ModifyDate { get; set; }
}
