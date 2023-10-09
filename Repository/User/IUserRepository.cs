using Kiselev.UserDbTestTask.Data.Users;

namespace Kiselev.UserDbTestTask.Repository.User
{
    public interface IUserRepository : IDisposable
    {
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO?> GetUserAsync(int userId);
        Task<Result> TryAddUserAsync(UserDTO user);
        Task<Result> TryUpdateUserAsync(UserDTO user);
        Task<bool> TryDeleteUserAsync(int userId);

        Task SaveAsync();
    }
}
