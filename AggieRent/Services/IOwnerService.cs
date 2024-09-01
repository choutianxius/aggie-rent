using AggieRent.Models;

namespace AggieRent.Services
{
    public interface IOwnerService
    {
        Owner? GetOwnerById(string id);

        IEnumerable<Owner> GetOwners();

        string CreateOwner(string email, string password, string name, string? description);

        void UpdateOwner(string id, string? name, string? description);
        void ResetOwnerEmail(string id, string newEmail);
        void ResetOwnerPassword(string id, string newPassword);

        void DeleteOwner(string id);
    }
}
