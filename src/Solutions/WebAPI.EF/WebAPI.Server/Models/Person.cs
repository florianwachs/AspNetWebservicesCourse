using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPI.Server.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Age { get; set; }
        [Required]
        public string Name { get; set; }
    }
}