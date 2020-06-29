using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample1.Domain.Models
{
    public class Author
    {
        // 👇 Sollen Ids nicht automatisch erzeugt werden, kann dieses Attribute dies verhindern.
        // Standardmäßig wird eine Identity-Spalte mit Autoincrement in der DB angelegt.
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        // 👇 Validierungsattribute können verwendet werden, um die Konsistenz der Daten sicherzustellen.
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Range(0, 150)]
        public int Age { get; set; }

        // 👇 Relationen werden unterstützt, komplizierte Fälle wie many-to-many (Books) müssen aber noch im DbContext nachkonfiguriert werden
        public ICollection<ContactInfo> ContactInfos { get; set; }
        public ICollection<BookAuthorRel> Books { get; set; }
    }

}
