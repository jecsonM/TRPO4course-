using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public int? RoleId { get; set; }

    public string Login { get; set; } = null!;

    public byte[]? PasswordHash { get; set; }

    public virtual ICollection<Machineservice> Machineservices { get; set; } = new List<Machineservice>();

    public virtual ICollection<Relevantcost> Relevantcosts { get; set; } = new List<Relevantcost>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Serviceprovision> Serviceprovisions { get; set; } = new List<Serviceprovision>();
}
