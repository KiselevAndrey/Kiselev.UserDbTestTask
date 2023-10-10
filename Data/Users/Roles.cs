namespace Kiselev.UserDbTestTask.Data.Users
{
    public enum Roles { User, Admin, Support, SuperAdmin }

    public static class RoleExtension
    {
        private static readonly int _minValueMask, _maxValueMask;

        static RoleExtension()
        {
            var roles = Enum.GetValues<Roles>();
            _minValueMask = 1 << (int)roles.First();
            _maxValueMask = 1 << (int)roles.Last();
        }

        public static int ToMask(List<Roles> roles)
        {
            int mask = 0;
            foreach (var role in roles)
                mask += 1 << (int)role;

            return mask;
        }

        public static int ToMask(Roles role) =>
            1 << (int)role;

        public static List<Roles> FromMask(int mask)
        {
            var findingRoles = new List<Roles>();
            foreach (var role in Enum.GetValues<Roles>())
            {
                if(CheckRoleInMask(role, mask))
                    findingRoles.Add(role);
            }

            return findingRoles;
        }

        public static bool CheckRoleInMask(Roles role, int mask)
        {
            var roleMask = 1 << (int)role;
            return (mask & roleMask) == roleMask;
        }

        public static bool CheckValueInMask(int value) =>
            value >= _minValueMask && value <= _maxValueMask;

        public static (int min, int max) GetMaskRange() =>
            (_minValueMask, _maxValueMask);
    }
}
