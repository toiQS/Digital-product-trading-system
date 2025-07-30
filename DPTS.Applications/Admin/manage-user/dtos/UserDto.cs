namespace DPTS.Applications.Admin.manage_user.dtos
{
    public class UserDto
    {
        public int Count { get; set; }
        public List<UserIndexDto> Users { get; set; } = new List<UserIndexDto>();
    }
    public class UserIndexDto
    {
        public string Id{ get; set; }
        public string Image {  get; set; }
        public string Name { get; set; }
        public DateTime JoinAt { get; set; }
        public int CountOrder { get; set; }
        public string Status { get; set; }
        public decimal Expenditure { get; set; }
    }
}
