using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Relevantrequeststate
{
    public int RelevantRequestStateId { get; set; }

    public int RequestId { get; set; }

    public int RequestStateId { get; set; }

    public DateTime SetDate { get; set; }

    public virtual Request Request { get; set; } = null!;

    public virtual Requeststate RequestState { get; set; } = null!;
}
