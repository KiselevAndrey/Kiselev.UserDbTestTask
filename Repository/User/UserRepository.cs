using Kiselev.UserDbTestTask.Data.Users;
using Kiselev.UserDbTestTask.DataBase;

namespace Kiselev.UserDbTestTask.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDb _userDb;

        private bool _disposed;

        public UserRepository(UserDb userDb) =>
            _userDb = userDb;

        ~UserRepository() =>
            Dispose(false);

        public Task<List<UserModel>> GetUsersAsync()
        {
            var usersDTO = _userDb.Users;
            var models = new List<UserModel>();
            foreach (var user in usersDTO)
                models.Add(user.MapToModel());

            return Task.FromResult(models);
        }

        public async Task<UserModel?> GetUserAsync(int userId)
        {
            var userDTO = await _userDb.Users.FindAsync(new object[] {userId});
            return userDTO?.MapToModel();
        }

        public async Task AddUserAsync(UserModel user) =>
            await _userDb.Users.AddAsync(user.MapToDTO());
        public async Task AddUserAsync(UserDTO user) =>
            await _userDb.Users.AddAsync(user);

        public async Task<bool> TryUpdateUserAsync(UserModel user) =>
            await TryUpdateUserAsync(user.MapToDTO());
        public async Task<bool> TryUpdateUserAsync(UserDTO user)
        {
            var userDto = await _userDb.Users.FindAsync(new object[] { user.Id });
            if (userDto == null)
                return false;

            userDto.CopyFrom(user);
            return true;
        }

        public async Task<bool> TryDeleteUserAsync(int userId)
        {
            var userDto = await _userDb.Users.FindAsync(new object[] { userId });
            if (userDto == null)
                return false;

            _userDb.Users.Remove(userDto);
            return true;
        }

        public async Task SaveAsync() =>
            await _userDb.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) return;
            if (disposing)
                _userDb.Dispose();
            _disposed = true;
        }
    }
}
