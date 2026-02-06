using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public int? RoleId { get; set; }

    public string Login { get; set; } = null!;

    public byte[]? PasswordHash { get; set; } = null!;

    public virtual ICollection<MachineService> MachineServices { get; set; } = new List<MachineService>();

    public virtual ICollection<RelevantCost> RelevantCosts { get; set; } = new List<RelevantCost>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<ServiceProvision> ServiceProvisions { get; set; } = new List<ServiceProvision>();
}
