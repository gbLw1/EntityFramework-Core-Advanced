using EFCore.UoWRepository.Domain;

namespace EFCore.UoWRepository.Data.Repositories
{
    public interface IDepartamentoRepository
    {
        Task<Departamento> GetByIdAsync(int id);
        void Add(Departamento departamento);
        //bool Save();
    }
}
