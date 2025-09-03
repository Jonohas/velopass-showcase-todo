using Domain;

namespace Domain.Services;

public interface ITodoService
{
    Task<List<Todo>> GetAll(CancellationToken cancellationToken);
    Task<Todo?> Get(Guid id, CancellationToken cancellationToken);
    Task<Todo> Create(string name, bool done, CancellationToken cancellationToken);
    Task<bool> Update(Guid id, string name, bool done, CancellationToken cancellationToken);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);
}
