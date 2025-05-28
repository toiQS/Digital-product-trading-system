using DPTS.Domains;

namespace DPTS.Applications.Shareds
{
    public class EnumHandle
    {
        public EnumHandle() { }
        public string ParseEnumToString(StatusEntity enumValue)
        {
            switch (enumValue)
            {
                case StatusEntity.None:
                    return "Không chọn";
                case StatusEntity.Newest:
                    return "Mới nhất";
                case StatusEntity.BestSeller:
                    return "Bán chạy nhất";
                case StatusEntity.Pending:
                    return "Đang chờ";
                case StatusEntity.Available:
                    return "Cho phép";
                case StatusEntity.Block:
                    return "Chặn";
                case StatusEntity.Done:
                    return "Hoàn thành";
                case StatusEntity.Comfirmed:
                    return "Xác nhận thành công";
                case StatusEntity.Cancel:
                    return "Hủy";
                case StatusEntity.Resolved:
                    return "Đã giải quyết";
                case StatusEntity.Complaint:
                    return "Khiếu nại";
                case StatusEntity.FundsInEscrow:
                    return "Kinh phí trong ký quỹ";
                case StatusEntity.Resolving:
                    return "Đang giải quyết";
                default:
                    return "Không xác định";
            }
        }
    }
}
