using System.Diagnostics;
using System.Runtime.CompilerServices;
using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Repos;

public interface IRepo<T> where T : IEntity
{
    Task<T?> FindById(Guid id);

    Task Insert(T entity);
}

public abstract class Repo<T> : IRepo<T> where T : class, IEntity
{
    protected readonly SchneesporttageContext Context;

    protected ActivitySource ActivitySource { get; } = new("ml.schneesporttage.api.development");

    private Activity? StartActivity([CallerMemberName] String caller = "")
    {
        return ActivitySource.StartActivity($"{GetType().Name}.{caller}");
    }
    
    private DbSet<T> GetSet()
    {
        return Context.Set<T>();
    }

    protected Repo(SchneesporttageContext context)
    {
        Context = context;
    }

    public Task<T?> FindById(Guid id)
    {
        using var activity = StartActivity();
        
        return GetSet().FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task Insert(T entity)
    {
        using var activity = StartActivity();
        
        await GetSet().AddAsync(entity);
        await Context.SaveChangesAsync();
    }
}