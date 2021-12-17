namespace EFCore.Multitenant.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetTenantId(this HttpContext httpContext)
        {
            // rota com split: desenvolvedor.io/tenant-1/product → " " / "tenant-1" / "product"
            var tenant = httpContext.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];

            return tenant;
        }
    }
}
