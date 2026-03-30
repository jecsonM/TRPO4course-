using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Request
{
    public int RequestId { get; set; }

    public int ClientId { get; set; }

    public DateTime CreationDate { get; set; }

    public string ServiceAddress { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
