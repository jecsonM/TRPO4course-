using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Machine
{
    public int MachineId { get; set; }

    public int ClientId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? MastersComment { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
