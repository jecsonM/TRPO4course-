namespace MachineServices.ViewModels
{
    public class ServiceListViewModel
    {
        public int ServiceId { get; set; }
        public string MachineServiceName { get; set; } = null!;
        public decimal CurrentPrice { get; set; }
        public DateTime PriceSetDate { get; set; }
    }
}
