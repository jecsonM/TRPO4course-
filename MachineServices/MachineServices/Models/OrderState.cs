using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class OrderState
{
    public int OrderStateId { get; set; }

    public string OrderStateName { get; set; } = null!;

    public virtual ICollection<RelevantOrderState> RelevantOrderStates { get; set; } = new List<RelevantOrderState>();
}
