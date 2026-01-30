using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Request
{
    public int RequestId { get; set; }

    public int ClientId { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public string ServiceAddress { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<RelevantRequestState> RelevantRequestStates { get; set; } = new List<RelevantRequestState>();
}
