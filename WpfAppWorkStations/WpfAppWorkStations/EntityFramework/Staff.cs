using System;
using System.Collections.Generic;

namespace WpfAppWorkStations.EntityFramework;

public partial class Staff
{
    public int StaffId { get; set; }

    public int? RoleId { get; set; }

    public string Login { get; set; } = null!;

    public byte[]? PasswordHash { get; set; }

    public virtual ICollection<Machineservice> Machineservices { get; set; } = new List<Machineservice>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Serviceprovision> Serviceprovisions { get; set; } = new List<Serviceprovision>();
}
