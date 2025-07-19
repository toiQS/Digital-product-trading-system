namespace DPTS.Applications.shareds
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
        public required T Data { get; set; }
        public string MessageResult { get; set; } = string.Empty;

        public static ServiceResult<T> Success(T data)
        {

            if (data is System.Collections.IEnumerable enumerable)
            {
                bool hasData = enumerable.Cast<object>().Any();

                var result = new ServiceResult<T>
                {
                    Status = hasData ? StatusResult.Success : StatusResult.Warning,
                    Data = data,

                };
                result.MessageResult = result.Status == StatusResult.Success ? $"{ServiceResultHandler(result.Status)}" : $"{ServiceResultHandler(result.Status)}: Không tìm thấy bất cứ dữ liệu nào.";

                return result;
            }


            if (data == null || data.Equals(default(T)))
            {
                return new ServiceResult<T>
                {
                    Status = StatusResult.Failed,
                    Data = default!,
                    MessageResult = $"{ServiceResultHandler(StatusResult.Failed)}: Không tìm thấy dữ liệu."
                };
            }

            return new ServiceResult<T>
            {
                Status = StatusResult.Success,
                Data = data,
                MessageResult = ServiceResultHandler(StatusResult.Success)
            };
        }

        public static ServiceResult<T> Error(string message)
        {
            return new ServiceResult<T>
            {
                Data = default!,
                MessageResult = $"{ServiceResultHandler(StatusResult.Errored)}: {message}",
                Status = StatusResult.Errored
            };
        }

        private static string ServiceResultHandler(Enum @enum)
        {
            switch (@enum)
            {
                case StatusResult.Success:
                    return "Thành công.";
                case StatusResult.Failed:
                    return "Thất bại";
                case StatusResult.Errored:
                    return "Xảy ra lỗi";
                case StatusResult.Warning:
                    return "Cảnh báo";
                default:
                    return "Không xác định.";
            }
        }
    }
}
