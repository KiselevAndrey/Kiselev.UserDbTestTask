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
            var checkUserResult = await CheckUserToCreate(user);
            if (checkUserResult.IsSuccess == false)
                return checkUserResult;
            
            user.Id = 0;
            await _userDb.Users.AddAsync(user);

            return new Result(true);
        }

        public async Task<Result> TryUpdateUserAsync(UserDTO user)
        {
            var userDto = await _userDb.Users.FindAsync(new object[] { user.Id });
            if (userDto == null)
                return new Result(false, $"User with id ({user.Id}) not found");
           
            var checkUserResult = await CheckUserToUpdate(userDto, user);
            if (checkUserResult.IsSuccess == false)
                return checkUserResult;

            userDto.CopyFrom(user);
            return new Result(true);
        }

        public async Task<Result> TryAddNewRoleAsync(int userId, Roles role)
        {
            var userDto = await _userDb.Users.FindAsync(new object[] { userId });
            if (userDto == null)
                return new Result(false, $"User with id ({userId}) not found");

            if (RoleExtension.CheckRoleInMask(role, userDto.RoleMask))
                return new Result(false, $"User with id ({userId}) already has role ({role})");

            userDto.RoleMask += RoleExtension.ToMask(role);
            return new Result(true);
        }

        public async Task<bool> TryDeleteUserAsync(int userId)
        {
            var userDto = await _userDb.Users.FindAsync(new object[] { userId });
            if (userDto == null)
                return false;

            _userDb.Users.Remove(userDto);
            return true;
        }

        public async Task<bool> TryDeleteUsersRoleAsync(int userId)
        {
            var userDto = await _userDb.Users.FindAsync(new object[] { userId });
            if (userDto == null)
                return false;

            userDto.RoleMask = 0;
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

        private async Task<Result> CheckUserToCreate(UserDTO user)
        {
            var userFullnessResult = CheckUserFullness(user);
            if(userFullnessResult.IsSuccess == false)
                return userFullnessResult;

            if (await CheckEmailUniq(user.Email) == false)
                return new Result(false, "User's email is already exist");

            return new Result(true);
        }

        private async Task<Result> CheckUserToUpdate(UserDTO oldUser, UserDTO newUser)
        {
            var userFullnessResult = CheckUserFullness(newUser);
            if (userFullnessResult.IsSuccess == false)
                return userFullnessResult;

            if (oldUser.Email != newUser.Email
                && await CheckEmailUniq(newUser.Email) == false)
                return new Result(false, "User's email is already exist");

            return new Result(true);
        }

        private static Result CheckUserFullness(UserDTO user)
        {
            if (string.IsNullOrEmpty(user.Name))
                return new Result(false, "User's name is null or Empty");
            if (string.IsNullOrEmpty(user.Email))
                return new Result(false, "User's email is null or Empty");
            if (user.Age < 1)
                return new Result(false, "User's age is less than 1");
            if (RoleExtension.CheckValueInMask(user.RoleMask) == false)
            {
                var (min, max) = RoleExtension.GetMaskRange();
                return new Result(false, $"User's role ({user.RoleMask}) is less than {min} or more than {max}");
            }

            return new Result(true);
        }

        private async Task<bool> CheckEmailUniq(string newEmail)
        {
            var usersDTO = await _userDb.Users.ToListAsync();
            bool emailIsUniq = true;
            for (int i = 0; i < usersDTO.Count; i++)
            {
                if (usersDTO[i].Email == newEmail)
                {
                    emailIsUniq = false;
                    break;
                }
            }

            return emailIsUniq;
        }
    }
}
