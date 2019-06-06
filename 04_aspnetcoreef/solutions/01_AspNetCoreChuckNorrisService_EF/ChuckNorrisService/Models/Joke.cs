using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ChuckNorrisService.Models
{
    public class Joke
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        [Required]
        [StringLength(500)]
        public string JokeText { get; set; }
        public ICollection<JokeCategory> Categories { get; set; }
    }

    public class JokeOnly
    {
        public string Id { get; set; }
        public string JokeText { get; set; }
    }

    public class JokeDto
    {
        public string Id { get; set; }
        public string JokeText { get; set; }
        public string[] Category { get; set; }
    }
}
