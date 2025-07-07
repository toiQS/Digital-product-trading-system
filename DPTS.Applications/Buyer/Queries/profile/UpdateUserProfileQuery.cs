using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.profile
{
    public class UpdateUserProfileQuery : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

}
