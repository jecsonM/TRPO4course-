using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Relevantorderstate
{
    public int RelevantOrderStateId { get; set; }

    public int OrderId { get; set; }

    public int OrderStateId { get; set; }

    public DateTime SetDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Orderstate OrderState { get; set; } = null!;
}
