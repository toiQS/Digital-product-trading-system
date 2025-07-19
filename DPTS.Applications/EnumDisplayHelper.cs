using DPTS.Domains;

namespace DPTS.Applications
{
    public static class EnumDisplayHelper
    {
        public static string ToDisplay(this ComplaintStatus status) => status switch
        {
            ComplaintStatus.Pending => "Đang chờ xử lý",
            ComplaintStatus.Resolved => "Đã xử lý",
            ComplaintStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this EscrowStatus status) => status switch
        {
            EscrowStatus.Pending => "Đang giữ tiền",
            EscrowStatus.Avalible => "Có thể rút",
            EscrowStatus.BeenComplainting => "Đang khiếu nại",
            EscrowStatus.Error => "Giao dịch lỗi",
            EscrowStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this AdjustmentType type) => type switch
        {
            AdjustmentType.Tax => "Thuế",
            AdjustmentType.Discount => "Giảm giá",
            AdjustmentType.PlatformFee => "Phí nền tảng",
            _ => "Không xác định"
        };

        public static string ToDisplay(this RuleStatus status) => status switch
        {
            RuleStatus.Active => "Đang áp dụng",
            RuleStatus.Inactive => "Tạm ngưng",
            RuleStatus.Expired => "Hết hạn",
            _ => "Không xác định"
        };

        public static string ToDisplay(this ParticipantType type) => type switch
        {
            ParticipantType.User => "Người dùng",
            ParticipantType.Store => "Cửa hàng",
            _ => "Không xác định"
        };

        public static string ToDisplay(this PaymentProvider provider) => provider switch
        {
            PaymentProvider.Vietcombank => "Vietcombank",
            PaymentProvider.MoMo => "MoMo",
            PaymentProvider.ZaloPay => "ZaloPay",
            _ => "Không xác định"
        };

        public static string ToDisplay(this ProductStatus status) => status switch
        {
            ProductStatus.Pending => "Chờ duyệt",
            ProductStatus.Available => "Đang bán",
            ProductStatus.Blocked => "Bị chặn",
            ProductStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this StoreStatus status) => status switch
        {
            StoreStatus.Active => "Hoạt động",
            StoreStatus.Inactive => "Tạm ngưng",
            StoreStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this TransactionType type) => type switch
        {
            TransactionType.Deposit => "Nạp tiền",
            TransactionType.Withdraw => "Rút tiền",
            TransactionType.Purchase => "Mua hàng",
            TransactionType.Refund => "Hoàn tiền",
            TransactionType.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this WalletTransactionStatus status) => status switch
        {
            WalletTransactionStatus.Pending => "Đang xử lý",
            WalletTransactionStatus.Completed => "Đã hoàn thành",
            WalletTransactionStatus.Failed => "Thất bại",
            WalletTransactionStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this OrderStatus status) => status switch
        {
            OrderStatus.Pending => "Đang xử lý",
            OrderStatus.WaitingConfirm => "Chờ người mua xác nhận",
            OrderStatus.BuyerConfirmed => "Người mua đã xác nhận",
            OrderStatus.Done => "Hoàn tất",
            OrderStatus.Complaint => "Khiếu nại",
            OrderStatus.Canceled => "Đã hủy",
            OrderStatus.Failed => "Lỗi",
            OrderStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string ToDisplay(this ReceiverType type) => type switch
        {
            ReceiverType.User => "Người dùng",
            ReceiverType.Store => "Cửa hàng",
            _ => "Không xác định"
        };
    }
}

