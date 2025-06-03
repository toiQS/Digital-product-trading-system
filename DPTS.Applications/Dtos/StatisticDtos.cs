using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DPTS.Applications.Dtos
{
    internal class StatisticDtos
    {
    }
    public class StatisticSellerIndexDto
    {
        public string StatisticName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Infomation { get; set; } = string.Empty;
    }
    public class StatisticBestSellerIndexDto
    {
        public string ProductName {  get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string Information {  get; set; } = string.Empty;
        public decimal Value { get; set; }

    }
    public class RecentMessageIndexDto
    {
        public string UserImage {  get; set; } = string.Empty;
        public string MessageId {  get; set; } = string.Empty;
        public string SenderName {  get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;
        public string SendAt {  get; set; } = string.Empty;
    }
    public class RecentOrderStatisticDto
    {
        public string OrderId {  get; set; } = string.Empty;
        public string BuyerImage {  get; set; } = string.Empty;
        public string BuyerName {  get; set; } = string.Empty;
        public string Information { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Status {  get; set; } = string.Empty;
    }
}
