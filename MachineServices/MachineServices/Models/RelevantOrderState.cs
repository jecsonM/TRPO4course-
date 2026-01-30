using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class RelevantOrderState
{
    public int RelevantOrderStateId { get; set; }

    public int OrderId { get; set; }

    public int OrderStateId { get; set; }

    public DateTimeOffset SetDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderState OrderState { get; set; } = null!;
}
