//namespace Karpinski_XY.Infrastructure.Services
//{
//    public class OperationResult<T> : OperationResult

//    {
//        protected OperationResult(T result, IReadOnlyCollection<string> errors = null)
//        {
//            Errors = errors ?? new List<string>();
//            ResultObject = result;
//        }

//        protected OperationResult(IReadOnlyCollection<string> errors)
//        {
//            Errors = errors ?? new List<string>();
//        }
        
//        protected OperationResult(string error)
//        {
//            Errors = new List<string> { error };
//        }

//        public T ResultObject { get; }

//        public static OperationResult<T> NotFound()
//        {
//            string error = ErrorMsgBuilder.BuildErrorNotFound<T>();
//            return new OperationResult<T>(error);
//        }

//        public static OperationResult<T> NotFound(string error)
//        {
//            return new OperationResult<T>(ErrorMsgBuilder.BuildErrorNotFound<T>(error));
//        }

//        public static OperationResult<T> Ok(T entity)
//        {
//            return new OperationResult<T>(entity);
//        }

//        public static OperationResult<T> Error(IReadOnlyCollection<string> errors)
//        {
//            return new OperationResult<T>(errors);
//        }

//    }


//    public class OperationResult
//    {
//        protected OperationResult()
//        {
//            Errors = new List<string>();
//        }

//        protected OperationResult(IReadOnlyCollection<string> errors = null)
//        {
//            Errors = errors ?? new List<string>();
//        }

//        public bool Success => !Errors.Any();

//        public IReadOnlyCollection<string> Errors { get; protected set; } = new List<string>();

//        public static OperationResult NoAction()
//        {
//            return new OperationResult();
//        }

//        public static OperationResult Error(string errorKey)
//        {
//            return new OperationResult(new List<string> { errorKey });
//        }

//    }
//}
