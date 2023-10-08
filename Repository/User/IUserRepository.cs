using Kiselev.UserDbTestTask.Data.Users;

namespace Kiselev.UserDbTestTask.Repository.User
{
    public interface IUserRepository : IDisposable
    {
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel?> GetUserAsync(int userId);
        Task AddUserAsync(UserModel user);
        Task AddUserAsync(UserDTO user);
        Task<bool> TryUpdateUserAsync(UserModel user);
        Task<bool> TryUpdateUserAsync(UserDTO user);
        Task<bool> TryDeleteUserAsync(int userId);

        Task SaveAsync();
    }
}
