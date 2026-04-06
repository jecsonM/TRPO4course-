using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int RequestId { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Relevantorderstate> Relevantorderstates { get; set; } = new List<Relevantorderstate>();

    public virtual Request Request { get; set; } = null!;

    public virtual ICollection<Serviceprovision> Serviceprovisions { get; set; } = new List<Serviceprovision>();

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
