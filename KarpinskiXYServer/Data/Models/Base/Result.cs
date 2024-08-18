namespace Karpinski_XY_Server.Data.Models.Base
{
    public class Result<T>
    {
        public Result()
        {
            Errors = new List<string>();
        }

        public bool Succeeded { get; private set; }
        public bool Failure => !Succeeded;
        public T Value { get; private set; }
        public IReadOnlyList<string> Errors { get; private set; }

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                Succeeded = true,
                Value = value
            };
        }

        public static Result<T> Fail(IEnumerable<string> errors)
        {
            return new Result<T>
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }

        public static Result<T> Fail(string error)
        {
            return Fail(new List<string> { error });
        }
    }
}
