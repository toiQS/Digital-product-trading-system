namespace DPTS.Applications.Shareds
{
    public enum StatusResult
    {
        Success,
        Warning,
        Failed,
        Errored,
    }
    public class ServiceResult<T>
    {
        public StatusResult Status { get; set; }
        public  T? Data { get; set; }
        public string MessageResult { get; set; } = string.Empty;

        public static ServiceResult<T> Success(T data)
        {
            
            if (data is System.Collections.IEnumerable enumerable)
            {
                bool hasData = enumerable.Cast<object>().Any();

                return new ServiceResult<T>
                {
                    Status = hasData ? StatusResult.Success : StatusResult.Warning,
                    Data = data,
                    MessageResult = hasData ? nameof(StatusResult.Success) : "Not Found"
                };
            }

            
            if (data == null || data.Equals(default(T)))
            {
                return new ServiceResult<T>
                {
                    Status = StatusResult.Failed,
                    Data = default!,
                    MessageResult = "Not Found"
                };
            }

            return new ServiceResult<T>
            {
                Status = StatusResult.Success,
                Data = data,
                MessageResult = nameof(StatusResult.Success)
            };
        }

        public static ServiceResult<T> Error(string message)
        {
            return new ServiceResult<T>
            {
                Data = default,
                MessageResult = message,
                Status = StatusResult.Errored
            };
        }
        //public static ServiceResult<T> f
    }
}
