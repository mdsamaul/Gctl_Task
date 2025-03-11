using System;
using System.Collections.Generic;

namespace GtclTaskProject.Models;

public partial class HrmAtdRosterScheduleEntry
{
    public decimal AiId { get; set; }

    public string RosterScheduleCode { get; set; } = null!;

    public string EmployeeId { get; set; } = null!;

    public DateTime Date { get; set; }

    public int ShiftCode { get; set; }

    public string Remarks { get; set; } = null!;

    public DateTime? EntryDate { get; set; }

    public DateTime? ModifyDate { get; set; }
}
