using EFCore.Multitenant.Domain.Abstract;

namespace EFCore.Multitenant.Domain
{
    public class Product : BaseEntity
    {
        public string? Description { get; set; }
    }
}
