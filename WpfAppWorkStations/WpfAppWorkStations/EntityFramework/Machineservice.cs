using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Machineservice
{
    public int ServiceId { get; set; }

    public int? CreatorsId { get; set; }

    public string MachineServiceName { get; set; } = null!;

    public virtual Staff? Creators { get; set; }

    public virtual ICollection<Relevantcost> Relevantcosts { get; set; } = new List<Relevantcost>();

    public virtual ICollection<Serviceprovision> Serviceprovisions { get; set; } = new List<Serviceprovision>();
    public Relevantcost CurrentPrice { get => Relevantcosts.OrderByDescending(rc => rc.SetDate).FirstOrDefault(); }
}
