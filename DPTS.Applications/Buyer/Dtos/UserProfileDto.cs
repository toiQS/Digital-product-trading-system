namespace DPTS.Applications.Buyer.Dtos
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = string.Empty;

        // Hồ sơ
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        // Địa chỉ
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

}
