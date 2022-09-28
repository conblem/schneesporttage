using api.Entities;

namespace api.Repos;

public interface IUserRepo: IRepo<User> {}

public class UserRepo : Repo<User>, IUserRepo
{
    public UserRepo(SchneesporttageContext context) : base(context)
    {
    }
}