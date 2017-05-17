using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.EF.Models
{
    public class Student
    {
        // Per Convention wird dieses Feld im EF
        // zum Id-Feld
        // Mit dem [Key]-Attribute kann es auch explizit angegeben werden
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Range(0, 10)]
        public decimal Motivation { get; set; }

        public DateTime Joined { get; set; }
        // Es gibt spezialisierte Validatoren.
        // Man kann auch seine eigenen erstellen.
        [EmailAddress]
        public string Email { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        // Relationen
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
