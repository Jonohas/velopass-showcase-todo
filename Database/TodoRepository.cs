using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Database;

public class TodoRepository : EFRepository<Todo, Guid, TodoDbContext>, ITodoRepository
{
    public TodoRepository(TodoDbContext dbContextFactory) : base(dbContextFactory)
    {
    }

}