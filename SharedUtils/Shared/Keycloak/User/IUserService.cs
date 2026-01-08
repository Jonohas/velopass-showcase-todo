using JV.ResultUtilities;

namespace Shared.Keycloak.User;

public interface IUserService<T>
  where T : BaseUser
{
  Task<Result> AddAsync(Guid id, CancellationToken cancellationToken);
  Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
  Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken);
}
