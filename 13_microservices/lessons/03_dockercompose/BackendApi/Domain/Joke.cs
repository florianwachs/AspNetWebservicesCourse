using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.Domain
{
    public class Joke
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [Column(TypeName = "jsonb")]
        public JokeAuthor Author { get; set; }
    }

    public class JokeAuthor
    {
        public string Name { get; set; }
    }
}
