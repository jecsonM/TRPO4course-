using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int RequestId { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public virtual ICollection<RelevantOrderState> RelevantOrderStates { get; set; } = new List<RelevantOrderState>();

    public virtual Request Request { get; set; } = null!;

    public virtual ICollection<ServiceProvision> ServiceProvisions { get; set; } = new List<ServiceProvision>();

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
