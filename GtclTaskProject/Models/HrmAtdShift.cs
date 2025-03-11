using System;
using System.Collections.Generic;

namespace GtclTaskProject.Models;

public partial class HrmAtdShift
{
    public int ShiftCode { get; set; }

    public string ShiftName { get; set; } = null!;

    public string ShiftShortName { get; set; } = null!;

    public DateTime ShiftStartTime { get; set; }

    public DateTime ShiftEndTime { get; set; }

    public DateTime LateTime { get; set; }

    public DateTime AbsentTime { get; set; }

    public DateTime Wef { get; set; }

    public string Remarks { get; set; } = null!;

    public DateTime? EntryDate { get; set; }

    public DateTime? ModifyDate { get; set; }
}
