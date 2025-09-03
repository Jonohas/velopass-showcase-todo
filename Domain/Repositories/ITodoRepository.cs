using Shared;

namespace Domain.Repositories;

public interface ITodoRepository : IRepository<Todo, Guid>
{
    Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken);
}