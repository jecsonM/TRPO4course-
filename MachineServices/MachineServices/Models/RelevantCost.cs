using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class RelevantCost
{
    public int RelevantCostId { get; set; }

    public int ServiceId { get; set; }

    public int CreatorsId { get; set; }

    public decimal RelevantCost1 { get; set; }

    public DateTimeOffset SetDate { get; set; }

    public virtual Staff Creators { get; set; } = null!;

    public virtual MachineService Service { get; set; } = null!;
}
