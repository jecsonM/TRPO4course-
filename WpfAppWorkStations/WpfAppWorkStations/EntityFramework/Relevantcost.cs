using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Relevantcost
{
    public int Relevantcostid { get; set; }

    public int ServiceId { get; set; }

    public int CreatorsId { get; set; }

    public decimal RelevantCost1 { get; set; }

    public DateTime SetDate { get; set; }

    public virtual Staff Creators { get; set; } = null!;

    public virtual Machineservice Service { get; set; } = null!;
}
