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

        public Task<List<UserDTO>> GetUsersAsync() =>
            _userDb.Users.ToListAsync();

        public async Task<UserDTO?> GetUserAsync(int userId) =>
            await _userDb.Users.FindAsync(new object[] {userId});

        public async Task<Result> TryAddUserAsync(UserDTO user)
        {
            if (string.IsNullOrEmpty(user.Name))
                return new Result(false, "User's name is null or Empty");
            if (string.IsNullOrEmpty(user.Email))
                return new Result(false, "User's email is null or Empty");
            if (user.Age < 1)
                return new Result(false, "User's age is less than 1");

            var usersDTO = await _userDb.Users.ToListAsync();
            bool emailIsUniq = true;
            for (int i = 0; i < usersDTO.Count; i++)
            {
                if (usersDTO[i].Email == user.Email)
                {
                    emailIsUniq = false;
                    break;
                }
            }

            if (emailIsUniq == false)
                return new Result(false, "User's email is already exist");
            
            user.Id = 0;
            await _userDb.Users.AddAsync(user);

            return new Result(true);
        }

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
