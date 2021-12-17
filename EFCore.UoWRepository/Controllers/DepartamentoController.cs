using EFCore.UoWRepository.Data;
using EFCore.UoWRepository.Data.Repositories;
using EFCore.UoWRepository.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.UoWRepository.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartamentoController : ControllerBase
    {
        private readonly ILogger<DepartamentoController> _logger;
        private readonly IUnityOfWork _UnityOfWork;

        public DepartamentoController(ILogger<DepartamentoController> logger, IUnityOfWork UnityOfWork)
        {
            _logger = logger;
            _UnityOfWork = UnityOfWork;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            //var departamento = await _departamentoRepository.GetByIdAsync(id);
            var departamento = await _UnityOfWork.DepartamentoRepository.GetByIdAsync(id);

            return Ok(departamento);
        }

        [HttpPost]
        public IActionResult CreateDepartamento(Departamento departamento)
        {
            _UnityOfWork.DepartamentoRepository.Add(departamento);
            _UnityOfWork.Commit();

            return Ok(departamento);
        }
    }
}