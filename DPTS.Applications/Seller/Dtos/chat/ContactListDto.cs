namespace DPTS.Applications.Seller.Dtos.chat
{
    public class ContactListDto
    {
        public int Count { get; set; }
        public List<ContactListIndexDto> Contacts { get; set; } = new List<ContactListIndexDto>();
    }
    public class ContactListIndexDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }
}
