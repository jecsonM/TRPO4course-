namespace MachineServices.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int RequestId { get; set; }
        public string ServiceAddress { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public List<ServiceItemViewModel> Services { get; set; } = new List<ServiceItemViewModel>();
    }
}