using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Database;

public class TodoRepository(TodoDbContext dbContextFactory)
    : EFRepository<Todo, Guid, TodoDbContext>(dbContextFactory), ITodoRepository
{
    public Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Set.ToListAsync(cancellationToken);
    }
}