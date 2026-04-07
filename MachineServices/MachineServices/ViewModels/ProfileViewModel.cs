using MachineServices.Models;

namespace MachineServices.ViewModels
{
    public class ProfileViewModel
    {
        public Client Client { get; set; } = null!;
        public List<RequestViewModel> Requests { get; set; } = new List<RequestViewModel>();
        public CreateRequestViewModel NewRequest { get; set; } = new CreateRequestViewModel();
    }
}