using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Request
{
    public int RequestId { get; set; }

    public int ClientId { get; set; }

    public DateTime CreationDate { get; set; }

    public string ServiceAddress { get; set; } = null!;

    public int? MasterId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Staff? Master { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Relevantrequeststate> Relevantrequeststates { get; set; } = new List<Relevantrequeststate>();
}
