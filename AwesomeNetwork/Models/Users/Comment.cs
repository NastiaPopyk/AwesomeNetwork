using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeNetwork.Models.Users
{
    [Table("Comment")]
    public class Comment
    {
        public int Id { get; set; }
        [Display(Name = "Коментар")]
        public string Text { get; set; }

        [Display(Name = "Дата створення")]
        public DateTime CreationDate { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public Comment() 
        {
            CreationDate = DateTime.Now;
        }
    }
}
