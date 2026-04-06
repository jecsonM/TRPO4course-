using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Serviceprovision
{
    public int OrderId { get; set; }

    public int ServiceId { get; set; }

    public int MastersId { get; set; }

    public int Amount { get; set; }

    public virtual Staff Masters { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Machineservice Service { get; set; } = null!;
}
