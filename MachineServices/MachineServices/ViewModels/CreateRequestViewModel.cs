using System.ComponentModel.DataAnnotations;

namespace MachineServices.ViewModels
{
    public class CreateRequestViewModel
    {
        
        [Required(ErrorMessage = "Введите адрес обслуживания")]
        [Display(Name = "Адрес обслуживания")]
        public string ServiceAddress { get; set; } = null!;
        
    }
}
