using Domain;

namespace Domain.Services;

public interface ITodoService
{
    Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken);
    Task<Todo?> Get(Guid id, CancellationToken cancellationToken);
    Task<Todo> CreateAsync(string name, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, string? name, bool? done, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
