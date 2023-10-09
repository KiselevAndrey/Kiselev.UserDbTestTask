namespace Kiselev.UserDbTestTask.Data.Users
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Roles { get; set; }

        public void CopyFrom(UserDTO other)
        {
            Name = other.Name;
            Age = other.Age;
            Email = other.Email;
            Roles = other.Roles;
        }
    }
}
