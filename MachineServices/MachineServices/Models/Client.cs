using System;
using System.Collections.Generic;

namespace MachineServices.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string ContactPersonFullname { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MainAddress { get; set; } = null!;

    public string Inn { get; set; } = null!;

    public string Kpp { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
