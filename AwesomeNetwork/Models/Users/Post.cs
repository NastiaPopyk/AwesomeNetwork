using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeNetwork.Models.Users
{
    [Table("Post")]
    public class Post
    {
        public int Id { get; set; }
        [Display(Name = "Заголовок")]
        public string Title { get; set; }
        [Display(Name = "Текст допису")]
        public string Description { get; set; }
        [Display(Name = "Дата створення")]
        public DateTime CreationDate { get; set; }
        public User User { get; set; }
        [Display(Name = "Тип допомоги")]
        public string PostType { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public Post() 
        {
            CreationDate = DateTime.Now;
        }
    }
}
