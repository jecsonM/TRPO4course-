using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class MachineService
{
    public int ServiceId { get; set; }

    public int? CreatorsId { get; set; }

    public string MachineServiceName { get; set; } = null!;

    public virtual Staff? Creators { get; set; }

    public virtual ICollection<RelevantCost> RelevantCosts { get; set; } = new List<RelevantCost>();

    public virtual ICollection<ServiceProvision> ServiceProvisions { get; set; } = new List<ServiceProvision>();
}
