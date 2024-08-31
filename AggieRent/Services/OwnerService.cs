using AggieRent.DataAccess;
using AggieRent.Models;
using AggieRent.Services;

public class OwnerService(IOwnerRepository ownerRepository) : IOwnerService
{
    private readonly IOwnerRepository _ownerRepository = ownerRepository;

    public Owner? GetOwnerById(string id)
    {
        return _ownerRepository.Get(id);
    }

    public IEnumerable<Owner> GetOwners()
    {
        return _ownerRepository.GetAll();
    }

    public string CreateOwner(string email, string password, string name, string? description)
    {
        AuthUtils.ValidateRegistrationCredentials<Owner>(email, password, _ownerRepository);
        var id = Guid.NewGuid().ToString();
        var owner = new Owner()
        {
            Id = id,
            Email = AuthUtils.NormalizeEmail(email),
            HashedPassword = BC.HashPassword(password),
            Name = name,
        };
        _ownerRepository.Add(owner);
        return id;
    }

    public void DeleteOwner(string id)
    {
        var owner = _ownerRepository.Get(id) ?? throw new ArgumentException("Owner ID not found");
        _ownerRepository.Remove(owner);
    }

    public void UpdateOwner(string id, string? name, string? description) { }

    public void ResetOwnerEmail(string id, string newEmail)
    {
        throw new NotImplementedException("ResetOwnerEmail not implemented");
    }

    public void ResetOwnerPassword(string id, string newPassword)
    {
        throw new NotImplementedException("ResetOwnerPasswor not implemented");
    }
}
