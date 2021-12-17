using EFCore.UoWRepository.Data.Repositories;

namespace EFCore.UoWRepository.Data
{
    public interface IUnityOfWork : IDisposable
    {
        bool Commit();
        IDepartamentoRepository DepartamentoRepository { get; }
    }
}
