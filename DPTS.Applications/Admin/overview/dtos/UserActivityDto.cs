namespace DPTS.Applications.Admin.overview.dtos
{
    public class UserActivityDto
    {
        public List<UserActivityIndexDto> UserActivities { get; set; } = new List<UserActivityIndexDto>();
    }
    public class UserActivityIndexDto
    {
        public string Name  { get; set; }
        public decimal Value { get; set; }
    }
}
