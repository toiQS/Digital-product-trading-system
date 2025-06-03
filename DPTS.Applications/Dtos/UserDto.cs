namespace DPTS.Applications.Dtos
{

    public class ProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; }  = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string NumberPhone {  get; set; } = string.Empty;
        public AddressDto Address { get; set; } = null!;
    }
    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
    public class MiniProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
