using Domain.Repositories;

namespace Domain.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repo;

    public TodoService(ITodoRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Todo>> GetAll(CancellationToken cancellationToken = default)
    {
        return _repo.GetAll(cancellationToken);
    }

    public Task<Todo?> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return _repo.Get(id, cancellationToken);
    }

    public async Task<Todo> Create(string name, bool done, CancellationToken cancellationToken = default)
    {
        var todo = Todo.Create(name, done);
        await _repo.SaveAsync(todo, cancellationToken);
        return todo;
    }

    public async Task<bool> Update(Guid id, string name, bool done, CancellationToken cancellationToken = default)
    {
        var todo = await _repo.Get(id, cancellationToken);
        if (todo == null) return false;
        var updated = todo.Update(name, done);
        if (updated)
        {
            await _repo.SaveAsync(todo, cancellationToken);
        }
        return updated;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await _repo.Get(id, cancellationToken);
        if (todo == null) return false;
        _repo.Delete(todo);
        return true;
    }
}
