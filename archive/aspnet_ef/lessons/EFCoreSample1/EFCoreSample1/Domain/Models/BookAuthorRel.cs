using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample1.Domain.Models
{
    // 👋 EF Core unterstützt die automatische Generierung von m:n Relationen. Wenn gewünscht kann natürlich eine manuelle Mapping Tabelle erstellt werden
    // Daher legen wir hier eine Mapping-Tabelle an, welche wir im DBContext konfigurieren müssen.
    //public class BookAuthorRel
    //{
    //    public int BookId { get; set; }
    //    public Book Book { get; set; }

    //    public int AuthorId { get; set; }
    //    public Author Author { get; set; }

    //}
}
