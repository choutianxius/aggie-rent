using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IOwnerService
    {
        Owner? GetOwnerById(string id);

        IEnumerable<Owner> GetOwners(string id);

        string CreateOwner(string email, string password, string name, string? description);

        void UpdateOwner(
            string id,
            string? email,
            string? password,
            string? name,
            string? description
        );

        void DeleteOwner(string id);
    }
}
