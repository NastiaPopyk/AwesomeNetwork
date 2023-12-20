using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.ViewModels.Account
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Поле Ім'я потрібно заповнити")]
        [DataType(DataType.Text)]
        [Display(Name = "Ім'я", Prompt = "Введіть ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Поле Прізвище потрібно заповнити")]
        [DataType(DataType.Text)]
        [Display(Name = "Прізвище", Prompt = "Введіть Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Поле Email потрібно заповнити")]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "example@mail.com")]
        public string EmailReg { get; set; }

        [Required(ErrorMessage = "Поле Рік потрібно заповнити")]
        [Display(Name = "Рік", Prompt = "Рік")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "Поле День потрібно заповнити")]
        [Display(Name = "День", Prompt = "День")]
        public int? Date { get; set; }

        [Required(ErrorMessage = "Поле Місяць потрібно заповнити")]
        [Display(Name = "Місяць", Prompt = "Місяць")]
        public int? Month { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Поле Пароль потрібно заповнити")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введіть пароль")]
        [StringLength(100, ErrorMessage = "Поле {0} повинно мати мінімум {2} і максимум {1} символів.", MinimumLength = 5)]
        public string PasswordReg { get; set; }

        [Required(ErrorMessage = "Підтвердіть пароль")]
        [Compare("PasswordReg", ErrorMessage = "Паролі не співпадають")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердіть пароль", Prompt = "Введіть пароль повторно")]
        public string PasswordConfirm { get; set; }

        public string Login => EmailReg;
        public string Role { get; set; }
    }
}
