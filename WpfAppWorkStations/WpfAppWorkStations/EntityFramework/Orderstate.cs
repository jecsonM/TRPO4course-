using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Orderstate
{
    public int OrderStateId { get; set; }

    public string OrderStateName { get; set; } = null!;

    public virtual ICollection<Relevantorderstate> Relevantorderstates { get; set; } = new List<Relevantorderstate>();
}
