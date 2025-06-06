using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MessageService> _logger;
        public MessageService(ApplicationDbContext context, ILogger<MessageService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        // Lấy các tin nhắn gần nhất của người bán
        public async Task<ServiceResult<IEnumerable<RecentMessageIndexDto>>> GetRecentMessagesAsync(string sellerId, int pageNumber = 2, int pageSize = 10)
        {
            _logger.LogInformation("Fetching recent messages for seller with ID: {SellerId}, Page: {PageNumber}, PageSize: {PageSize}", sellerId, pageNumber, pageSize);
            try
            {
                var messages = await _context.Messages
                .Where(x => x.ReceiverId == sellerId)
                .OrderByDescending(x => x.CreatedAt)
                .DistinctBy(x => x.SenderId)
                .Include(x => x.Sender)
                .ToListAsync();

                var result = messages.Select(x => new RecentMessageIndexDto
                {
                    MessageId = x.MessageId,
                    SenderName = x.Sender.FullName ?? "N/A",
                    Content = x.Content,
                    SendAt = x.CreatedAt.ToString("hh:mm - dd/MM/yyyy"),
                    UserImage = x.Sender.ImageUrl
                });

                return ServiceResult<IEnumerable<RecentMessageIndexDto>>.Success(result.Skip((pageNumber - 1) * pageSize).Take(pageSize));
            }
            catch
            {
                _logger.LogError("An error occurred while fetching recent messages for seller with ID: {SellerId}", sellerId);
                return ServiceResult<IEnumerable<RecentMessageIndexDto>>.Error("Không thể lấy danh sách tin nhắn gần nhất.");
            }
        }
    }
}
