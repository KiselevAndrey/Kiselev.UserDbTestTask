namespace Kiselev.UserDbTestTask.Data.Users
{
    public static class UserMapper
    {
        public static UserModel MapToModel(this UserDTO userDTO)
        {
            var model = new UserModel
            {
                Id = userDTO.Id,
                Name = userDTO.Name,
                Age = userDTO.Age,
                Email = userDTO.Email,
                Role = new(userDTO.RoleValue)
            };

            return model;
        }

        public static UserDTO MapToDTO(this UserModel userModel)
        {
            var dto = new UserDTO
            {
                Id = userModel.Id,
                Name = userModel.Name,
                Age = userModel.Age,
                Email = userModel.Email,
                RoleValue = userModel.Role.Value
            };

            return dto;
        }
    }
}
