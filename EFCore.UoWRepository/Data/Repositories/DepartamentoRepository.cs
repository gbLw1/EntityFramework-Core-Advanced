using EFCore.UoWRepository.Domain;
using Microsoft.EntityFrameworkCore;

namespace EFCore.UoWRepository.Data.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<Departamento> _Dep;

        public DepartamentoRepository(ApplicationContext context)
        {
            _context = context;
            _Dep = _context.Set<Departamento>();
        }

        public void Add(Departamento departamento)
            => _Dep.Add(departamento);

        public async Task<Departamento> GetByIdAsync(int id)
            => await _Dep.Include(p => p.Colaboradores).FirstOrDefaultAsync(p => p.Id == id);

        //public bool Save()
        //    => _context.SaveChanges() > 0;
    }
}
