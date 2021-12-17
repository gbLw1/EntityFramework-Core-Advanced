using EFCore.Multitenant.Data;
using EFCore.Multitenant.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EFCore.Multitenant.Controllers
{
    [ApiController]
    [Route("{tenant}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetProduct")]
        public IEnumerable<Product> Get([FromServices]ApplicationContext db)
        {
            var products = db.Products.ToArray();

            return products;
        }
    }
}