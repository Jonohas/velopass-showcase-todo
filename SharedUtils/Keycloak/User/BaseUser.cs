using Shared.Domain;

namespace Shared.Keycloak.User;

public abstract class BaseUser : Entity<Guid>
{
  public string Email { get; protected set; } = null!;
  public string FirstName { get; protected set; } = null!;
  public string LastName { get; protected set; } = null!;
  public bool IsActive { get; protected set; }


  public void Deactivate()
  {
    IsActive = false;
  }

  public void Activate()
  {
    IsActive = true;
  }

  public void UpdateName(string firstName, string lastName)
  {
    FirstName = firstName;
    LastName = lastName;
  }
}
