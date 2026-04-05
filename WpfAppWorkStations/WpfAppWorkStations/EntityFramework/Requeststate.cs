using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Requeststate
{
    public int RequestStateId { get; set; }

    public string RequestStateName { get; set; } = null!;

    public virtual ICollection<Relevantrequeststate> Relevantrequeststates { get; set; } = new List<Relevantrequeststate>();
}
