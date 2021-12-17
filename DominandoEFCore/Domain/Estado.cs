using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Domain
{
    public class Estado
    {

        public int Id { get; set; }
        public string? Nome { get; set; }

        public Governador? Governador { get; set; }

        public ICollection<Cidade> Cidades { get;} = new List<Cidade>();
    }

    public class Governador
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int? Idade { get; set; }
        public string? Partido { get; set; }

        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
    }

    public class Cidade
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
    }
}
