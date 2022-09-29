using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Repos;

public interface IUserRepo : IRepo<User>
{
    Task<List<User>> FindByFirstName(string firstname);
}

public class UserRepo : Repo<User>, IUserRepo
{
    public UserRepo(SchneesporttageContext context, ITelemetryService telemetryService) : base(context, telemetryService)
    {
    }

    public Task<List<User>> FindByFirstName(string firstname)
    {
        using var activity = StartActivity();
        activity?.AddTag(nameof(firstname), firstname);

        return GetSet().Where(user => user.Firstname.Equals(firstname)).ToListAsync();
    }
}