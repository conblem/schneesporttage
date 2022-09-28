using api.Repos;

namespace api;

public static class RepoExtensions
{
    public static IServiceCollection AddRepos(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<IUserRepo>(sp =>
            new UserRepo(sp.GetRequiredService<SchneesporttageContext>())
        );
    }
}