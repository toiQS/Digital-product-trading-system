using DPTS.Domains;
using System.Formats.Asn1;
using System.Transactions;

public static class EnumHandle
{
    public static string HandleComplaintStatus(ComplaintStatus status)
    {
        return status switch
        {
            ComplaintStatus.Pending => "Đang chờ xử lý",
            ComplaintStatus.Unknown => "Không rõ",
            ComplaintStatus.Resolved => "Đã xử lý",
            _ => "Không xác định"
        };
    }

    public static string HandleEscrowStatus(EscrowStatus status)
    {
        return status switch
        {
            EscrowStatus.Unknown => "Không rõ",
            EscrowStatus.WaitingComfirm => "Chờ xác nhận",
            EscrowStatus.Comfirmed => "Đã xác nhận",
            EscrowStatus.Done => "Hoàn tất",
            EscrowStatus.Canceled => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public static string HandleProductStatus(ProductStatus status)
    {
        return status switch
        {
            ProductStatus.Unknown => "Không rõ",
            ProductStatus.Pending => "Chờ duyệt",
            ProductStatus.Available => "Có sẵn",
            ProductStatus.Blocked => "Bị chặn",
            _ => "Không xác định"
        };
    }

    //public static string HandleProductHagtag(ProductHagtag tag)
    //{
    //    return tag switch
    //    {
    //        ProductHagtag.None => "",
    //        ProductHagtag.Newest => "Mới nhất",
    //        ProductHagtag.BestSeller => "Bán chạy",
    //        ProductHagtag.StopSelling => "Ngừng kinh doanh",
    //        _ => "Không xác định"
    //    };
    //}

    public static string HandleWalletUnitCurrency(UnitCurrency unitCurrency)
    {
        return unitCurrency switch
        {
            UnitCurrency.USD => "USD",
            UnitCurrency.VND => "VND",
            _ => "Không xác định"
        };
    }
    public static string HandleTradeStatus(TradeStatus status)
    {
        return status switch
        {
            TradeStatus.Done => "Hoàn thành",
            TradeStatus.Resolving => "Đang xử lý",
            TradeStatus.Errored => "Xảy ra vấn đề",
            _ => "Không xác định"
        };
    }
}
