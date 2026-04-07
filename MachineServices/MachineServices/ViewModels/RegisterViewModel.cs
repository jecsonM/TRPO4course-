using System.ComponentModel.DataAnnotations;

namespace MachineServices.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите название компании")]
        [Display(Name = "Название компании")]
        public string CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Введите ФИО контактного лица")]
        [Display(Name = "ФИО контактного лица")]
        public string ContactPersonFullname { get; set; } = null!;

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        [MaxLength(15, ErrorMessage = "Телефон не может превышать 15 символов")]
        [Display(Name = "Контактный телефон")]
        public string ContactPhone { get; set; } = null!;

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Введите юридический адрес")]
        [Display(Name = "Юридический адрес")]
        public string MainAddress { get; set; } = null!;

        [Required(ErrorMessage = "Введите ИНН")]
        [MaxLength(12, ErrorMessage = "ИНН не может превышать 12 символов")]
        [MinLength(10, ErrorMessage = "ИНН должен содержать минимум 10 символов")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; } = null!;

        [Required(ErrorMessage = "Введите КПП")]
        [MaxLength(9, ErrorMessage = "КПП не может превышать 9 символов")]
        [Display(Name = "КПП")]
        public string Kpp { get; set; } = null!;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Примечания")]
        [MaxLength(500, ErrorMessage = "Примечания не могут превышать 500 символов")]
        public string? Notes { get; set; }
    }
}
