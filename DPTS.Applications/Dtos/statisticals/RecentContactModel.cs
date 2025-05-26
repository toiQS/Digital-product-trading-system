namespace DPTS.Applications.Dtos.statisticals
{
    public class RecentContactModel
    {
        public string ImagePath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Text {  get; set; } = string.Empty;
        public DateTime SendAt { get; set; }
    }
}
