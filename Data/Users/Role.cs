namespace Kiselev.UserDbTestTask.Data.Users
{
    public class Role
    {
        public int Value { get; private set; }

        public Role(int roleValue)
        {
            Value = roleValue;
        }
    }
}
