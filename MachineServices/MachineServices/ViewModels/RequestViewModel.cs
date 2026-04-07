namespace MachineServices.ViewModels
{
    public class RequestViewModel
    {
        public int RequestId { get; set; }
        public DateTime CreationDate { get; set; }
        public string ServiceAddress { get; set; } = null!;
        public DateTime? LastStateDate { get; set; }
        public string? LastStateName { get; set; }
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
    }
}