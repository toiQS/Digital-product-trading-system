namespace DPTS.Applications.Admin.manage_user.dtos
{
    public class UserDto
    {
        public List<UserIndexDto> Users {get; set; }
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
