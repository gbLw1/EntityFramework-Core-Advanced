using EFCore.Multitenant.Domain.Abstract;

namespace EFCore.Multitenant.Domain
{
    public class Person : BaseEntity
    {
        public string? Name { get; set; }
    }
}
