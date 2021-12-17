using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.UDFs
{
    public static class MinhasFuncoes
    {
        [DbFunction(name: "LEFT", IsBuiltIn = true)]
        public static string Left(string dados, int quantidade)
        {
            throw new NotImplementedException();
        }

        public static string LetrasMaiusculas(string dados)
        {
            throw new NotImplementedException();
        }

        public static string DateDiff(string identificador, DateTime? dataInicial, DateTime dataFinal)
        {
            throw new NotImplementedException();
        }

        #region [Registrando funções com Data Annotations]

        //public static void Registrar(ModelBuilder builder)
        //{
        //    var funcoes = typeof(MinhasFuncoes)
        //        .GetMethods()
        //        .Where(p => Attribute.IsDefined(p, typeof(DbFunctionAttribute)));

        //    foreach (var func in funcoes)
        //        builder.HasDbFunction(func);
        //}

        #endregion
    }
}
