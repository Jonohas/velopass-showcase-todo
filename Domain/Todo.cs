namespace Domain;

public class Todo : Entity<Guid>
{
    public string Name { get; private set; }
    public bool Done { get; private set; }
    
    private Todo(string name, bool done)
    {
        Id = Guid.NewGuid();
        Name = name;
        Done = done;
    }

    public static Todo Create(string name, bool done)
    {
        return new Todo(name, done);
    }

    public bool Update(string name, bool done)
    {
        Name = name;
        Done = done;

        return true;
    }
}