namespace Kiselev.UserDbTestTask.Repository
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Description { get; private set; }

        public Result(bool isSuccess, string description = "")
        {
            IsSuccess = isSuccess;
            Description = description;
        }
    }
}
