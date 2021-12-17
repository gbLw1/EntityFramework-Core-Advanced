using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Domain
{
    [Table("TabelaAtributos")]
    public class Atributo
    {
        [Key]
        public int Id { get; set; }

        [Column("MinhaDescricao", TypeName = "VARCHAR(100)")]
        public string? Descricao { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Observacao { get; set; }
    }
}
