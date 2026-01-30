using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class RequestState
{
    public int RequestStateId { get; set; }

    public string RequestStateName { get; set; } = null!;

    public virtual ICollection<RelevantRequestState> RelevantRequestStates { get; set; } = new List<RelevantRequestState>();
}
