﻿using DPTS.Domains;

namespace DPTS.Applications.Shareds
{
    public static class EnumHandle
    {
        public static string HandleComplaintStatus(ComplaintStatus status) => status switch
        {
            ComplaintStatus.Pending => "Đang chờ xử lý",
            ComplaintStatus.Unknown => "Không rõ",
            ComplaintStatus.Resolved => "Đã xử lý",
            _ => "Không xác định"
        };

        public static string HandleEscrowStatus(EscrowStatus status) => status switch
        {
            EscrowStatus.Unknown => "Không rõ",
            EscrowStatus.WaitingConfirm => "Chờ người mua xác nhận",
            EscrowStatus.BuyerConfirmed => "Người mua đã xác nhận",
            EscrowStatus.Done => "Hoàn tất",
            EscrowStatus.Canceled => "Đã huỷ",
            _ => "Không xác định"
        };

        public static string HandleAdjustmentType(AdjustmentType type) => type switch
        {
            AdjustmentType.Tax => "Thuế",
            AdjustmentType.Discount => "Giảm giá",
            AdjustmentType.PlatformFee => "Phí nền tảng",
            _ => "Không xác định"
        };

        public static string HandleRuleStatus(RuleStatus status) => status switch
        {
            RuleStatus.Active => "Đang áp dụng",
            RuleStatus.Inactive => "Tạm ngưng",
            RuleStatus.Expired => "Hết hạn",
            _ => "Không xác định"
        };

        public static string HandleParticipantType(ParticipantType type) => type switch
        {
            ParticipantType.User => "Người dùng",
            ParticipantType.Store => "Cửa hàng",
            _ => "Không xác định"
        };

        public static string HandlePaymentProvider(PaymentProvider provider) => provider switch
        {
            PaymentProvider.Vietcombank => "Vietcombank",
            PaymentProvider.MoMo => "MoMo",
            PaymentProvider.ZaloPay => "ZaloPay",
            _ => "Không xác định"
        };

        public static string HandleProductStatus(ProductStatus status) => status switch
        {
            ProductStatus.Unknown => "Không rõ",
            ProductStatus.Pending => "Chờ kiểm duyệt",
            ProductStatus.Available => "Đang bán",
            ProductStatus.Blocked => "Bị chặn",
            _ => "Không xác định"
        };

        public static string HandleStoreStatus(StoreStatus status) => status switch
        {
            StoreStatus.Active => "Hoạt động",
            StoreStatus.Inactive => "Tạm ngưng",
            StoreStatus.Unknown => "Không rõ",
            _ => "Không xác định"
        };

        public static string HandleTransactionType(TransactionType type) => type switch
        {
            TransactionType.Deposit => "Nạp tiền",
            TransactionType.Withdraw => "Rút tiền",
            TransactionType.Purchase => "Mua hàng",
            TransactionType.Refund => "Hoàn tiền",
            _ => "Không xác định"
        };
    }
}
