using EFCore.Multitenant.Extensions;
using EFCore.Multitenant.Provider;

namespace EFCore.Multitenant.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // cria uma instancia do TenantData
            var tenant = httpContext.RequestServices.GetService<TenantData>();

            // recupera o tenant que está acessando a aplicação
            tenant.TenantId = httpContext.GetTenantId();

            // faz a chamada do próximo middleware
            await _next(httpContext);
        }
    }
}
