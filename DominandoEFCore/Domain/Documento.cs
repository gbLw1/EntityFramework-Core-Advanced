using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Domain
{
    public class Documento
    {
        private string _cpf;

        public int Id { get; set; }

        public void SetCPF(string cpf)
        {
            // Validações
            if(string.IsNullOrWhiteSpace(cpf))
                throw new Exception("CPF Inválido");

            _cpf = cpf;
        }

        public string GetCPF() => _cpf;
    }
}
