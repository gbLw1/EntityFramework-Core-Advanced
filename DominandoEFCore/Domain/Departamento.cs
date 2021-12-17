using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Domain
{
    public class Departamento
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        public byte[] Image { get; set; }

        public List<Funcionario>? Funcionarios { get; set; }
    }
}
