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
        return _ownerRepository.GetAll().ToList();
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

    public void UpdateOwner(string id, string? name, string? description)
    {
        var owner = _ownerRepository.Get(id) ?? throw new ArgumentException("Owner ID not found");
        if (name != null)
        {
            if (name.Equals(""))
                throw new ArgumentException("Name cannot be empty");
            owner.Name = name;
        }
        if (description != null)
        {
            owner.Description = description;
        }
        _ownerRepository.Update(owner);
    }

    public void ResetOwnerEmail(string id, string newEmail)
    {
        var owner = _ownerRepository.Get(id) ?? throw new ArgumentException("Owner ID not found");
        if (!AuthUtils.ValidateEmail(newEmail))
            throw new ArgumentException("Invalid Email format");

        var normalizedEmail = AuthUtils.NormalizeEmail(newEmail);
        var existingOwner = _ownerRepository
            .GetAll()
            .FirstOrDefault(u => u.Email == normalizedEmail);
        if (existingOwner != null)
            throw new ArgumentException("Email already in use!");
        owner.Email = normalizedEmail;
        _ownerRepository.Update(owner);
    }

    public void ResetOwnerPassword(string id, string newPassword)
    {
        var owner = _ownerRepository.Get(id) ?? throw new ArgumentException("Owner ID not found");
        if (!AuthUtils.ValidatePassword(newPassword))
            throw new ArgumentException(
                "Invalid password! Password must be at least 8 symbols long, with at least 1 lower case character, 1 upper case character, 1 symbol and 1 number"
            );
        owner.HashedPassword = BC.HashPassword(newPassword);
        _ownerRepository.Update(owner);
    }
}
