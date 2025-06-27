using DPTS.Applications.Buyers.profiles.Dtos;
using DPTS.Applications.Buyers.profiles.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.profiles.Handles
{
    public class GetUserProfileMiniHandle : IRequestHandler<GetUserProfileMiniQuery, ServiceResult<UserProfileMiniDto>>
    {
        private readonly ILogger<GetUserProfileMiniHandle> _logger;
        private readonly IUserRepository _userRepository;
        public GetUserProfileMiniHandle(ILogger<GetUserProfileMiniHandle> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<UserProfileMiniDto>> Handle(GetUserProfileMiniQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("");
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogError("");
                    return ServiceResult<UserProfileMiniDto>.Error("");
                }
                var result = new UserProfileMiniDto()
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    ProfileImage = user.ImageUrl,
                };
                return ServiceResult<UserProfileMiniDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<UserProfileMiniDto>.Error("");
            }
        }
    }
}
