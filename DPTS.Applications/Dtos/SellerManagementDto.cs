using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DPTS.Applications.Dtos
{
    public class SellerManagementDto
    {
    }
    public class ReportIndexDto
    {
        public string StatisticName { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Information { get; set; } = string.Empty;
    }
    public class ProductBestSaleIndexDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public double Price { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
    public class OrderRecentIndexDto
    {
        public string Image {  get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Price { get; set; }
    }
    public class MessageRecentIndexDto
    {
        public string Image { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ReceiveAt { get; set; } = string.Empty;
    }
    public class ProductIndexDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Price { get; set; }
        public int QuantitySold { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
    public class OrderIndexDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string BuyerName { get; set;} = string.Empty;
        public double Price { get; set; } 
        public string Status { get; set; } = string.Empty;
        public DateOnly DateOrder { get; set; }
        public TimeOnly TimeOrder { get; set; }
    }
    public class ProductIndexMiniDto
    {
        public string Image { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public double Price { get; set; }
    }
    public class ComplaintIndexDto
    {
        public string ImageBuyer { get; set; } = string.Empty;
        public string ComplaintId { get; set; } = string.Empty;
        public string ComplaintName { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ComplaintAt { get; set; }  
    }
}
