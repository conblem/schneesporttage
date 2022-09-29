using System.Diagnostics;
using System.Runtime.CompilerServices;
using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Repos;

public interface IRepo<T> where T : IEntity
{
    Task<T?> FindById(Guid id);

    Task Insert(T entity);

    Task<List<T>> All();
}

public abstract class Repo<T> : IRepo<T> where T : class, IEntity
{
    protected readonly SchneesporttageContext Context;
    private readonly ActivitySource _activitySource;

    protected Repo(SchneesporttageContext context, ITelemetryService telemetryService)
    {
        Context = context;
        _activitySource = telemetryService.ActivitySource;
    }
    
    protected Activity? StartActivity([CallerMemberName] string caller = "")
    {
        var activity = _activitySource.StartActivity(GetType().Name);
        activity?.AddTag("method", caller);
        
        return activity;
    }
    
    protected DbSet<T> GetSet()
    {
        return Context.Set<T>();
    }


    public Task<T?> FindById(Guid id)
    {
        using var activity = StartActivity();
        activity?.AddTag(nameof(id), id);
        
        return GetSet().FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task Insert(T entity)
    {
        using var activity = StartActivity();
        
        await GetSet().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public Task<List<T>> All()
    {
        using var activity = StartActivity();

        return GetSet().ToListAsync();
    }
}