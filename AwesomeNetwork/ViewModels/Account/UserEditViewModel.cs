using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.ViewModels.Account
{
    public class UserEditViewModel
    {
        [Required]
        [Display(Name = "Ідентифікатор користувача")]
        public string UserId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Ім'я", Prompt = "Введіть Ім'я")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Прізвище", Prompt = "Введіть прізвище")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email", Prompt = "example.com")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата народження")]
        public DateTime BirthDate { get; set; }

        public string UserName => Email;

        [DataType(DataType.Text)]
        [Display(Name = "По батькові", Prompt = "Введіть по батькові")]
        public string MiddleName { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Фото", Prompt = "Посилання на картинку")]
        public string Image { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Статус", Prompt = "Введіть статус")]
        public string Status { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Про себе", Prompt = "Дані про себе")]
        public string About { get; set; }
    }
}
