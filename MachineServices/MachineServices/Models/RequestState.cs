using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Requeststate
{
    public int RequestStateId { get; set; }

    public string RequestStateName { get; set; } = null!;

    public virtual ICollection<Relevantrequeststate> Relevantrequeststates { get; set; } = new List<Relevantrequeststate>();
}
