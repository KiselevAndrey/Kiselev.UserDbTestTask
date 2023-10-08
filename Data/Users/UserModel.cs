namespace Kiselev.UserDbTestTask.Data.Users
{
    public struct UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
