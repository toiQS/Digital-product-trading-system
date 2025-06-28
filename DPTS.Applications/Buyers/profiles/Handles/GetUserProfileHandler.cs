using DPTS.Applications.Buyers.profiles.Dtos;
using DPTS.Applications.Buyers.profiles.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.profiles.Handles
{
    public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, ServiceResult<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserProfileHandler> _logger;
        public async Task<ServiceResult<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("");
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogError("");
                    return ServiceResult<UserProfileDto>.Error("");
                }
                var result = new UserProfileDto()
                {
                    //FullName  = user.FullName,
                    //PhoneNumber = user.Phone,
                    //City = user.Address.City,
                    //Country = user.Address.Country,
                    //District = user.Address.District,
                    //Email = user.Email,
                    //IsEmailVerified = user.TwoFactorEnabled,
                    //PostalCode = user.Address.PostalCode,
                    //Street = user.Address.Street,
                    //UserId = user.UserId,
                };
                return ServiceResult<UserProfileDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<UserProfileDto>.Error("");
            }
            throw new NotImplementedException();
        }
    }
}
