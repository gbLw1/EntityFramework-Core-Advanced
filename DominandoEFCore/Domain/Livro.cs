using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Domain
{
    public class Livro
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }

        [Column(TypeName = "VARCHAR(15)")]
        public string? Autor { get; set; }
        public DateTime? CadastradoEm { get; set; }
    }
}
