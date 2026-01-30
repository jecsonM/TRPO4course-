using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class RelevantRequestState
{
    public int RelevantRequestStateId { get; set; }

    public int RequestId { get; set; }

    public int RequestStateId { get; set; }

    public DateTimeOffset SetDate { get; set; }

    public virtual Request Request { get; set; } = null!;

    public virtual RequestState RequestState { get; set; } = null!;
}
