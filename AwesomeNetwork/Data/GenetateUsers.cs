using AwesomeNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Data
{
    public class GenetateUsers
    {
        public readonly string[] maleNames = new string[] { "Іван", "Петро", "Василь", "Євген", "Данило", "Григір", "Григорій", "Олексій", "Вадимчик", "Драгомир" };
        public readonly string[] femaleNames = new string[] { "Анна", "Марина", "Катерина", "Ізольда" };
        public readonly string[] lastNames = new string[] { "Розробник", "Кнопкоклац", "Тестер", "Зеленько", "Українець" };

        public List<User> Populate(int count)
        {
            var users = new List<User>();
            for (int i = 1; i < count; i++)
            {
                string firstName;
                var rand = new Random();

                var male = rand.Next(1, 2) == 1;

                var lastName = lastNames[rand.Next(0, lastNames.Length - 1)];
                if (male)
                {
                    firstName = maleNames[rand.Next(0, maleNames.Length - 1)];
                }
                else
                {
                    lastName = lastName + "a";
                    firstName = femaleNames[rand.Next(0, femaleNames.Length - 1)];
                }

                var item = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = DateTime.Now.AddDays(-rand.Next(1, (DateTime.Now - DateTime.Now.AddYears(-25)).Days)),
                    Email = "test" + rand.Next(0, 1204) + "@test.com",
                };

                item.UserName = item.Email;
                item.Image = "/Images/Default/user.png";

                users.Add(item);
            }

            return users;
        }
    }
}
