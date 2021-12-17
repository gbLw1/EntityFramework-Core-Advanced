using EFCore.UoWRepository.Data.Repositories;

namespace EFCore.UoWRepository.Data
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly ApplicationContext _context;
        private readonly IDepartamentoRepository _departamentoRepository;

        public UnityOfWork(ApplicationContext context, IDepartamentoRepository repository)
        {
            _context = context;
            _departamentoRepository = repository;
        }

        public IDepartamentoRepository DepartamentoRepository => _departamentoRepository;

        //private IDepartamentoRepository _departamentoRepository;
        //public IDepartamentoRepository DepartamentoRepository
        //{
        //    get => _departamentoRepository ?? (_departamentoRepository = new DepartamentoRepository(_context));
        //}

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
