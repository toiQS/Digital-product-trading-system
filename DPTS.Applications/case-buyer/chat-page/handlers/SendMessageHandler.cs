using DPTS.Applications.case_buyer.chat_page.dtos;
using DPTS.Applications.case_buyer.chat_page.models;
using DPTS.Applications.shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using DPTS.Infrastructures.Repositories.Contracts.Messages;
using DPTS.Infrastructures.Repositories.Contracts.Notifications;
using DPTS.Infrastructures.Repositories.Contracts.Stores;
using DPTS.Infrastructures.Repositories.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.case_buyer.chat_page.handlers
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, ServiceResult<ChatDto>>
    {
        private readonly IMessageCommand _messageCommand;
        private readonly IMediator _mediator;
        private readonly ILogger<SendMessageHandler> _logger;
        private readonly INotificationCommand _notificationCommand;
        private readonly IUserQuery _userQuery;
        private readonly IStoreQuery _storeQuery;
        private readonly ApplicationDbContext _context;

        public SendMessageHandler(IMessageCommand messageCommand,
                                  IMediator mediator,
                                  ILogger<SendMessageHandler> logger,
                                  INotificationCommand notificationCommand,
                                  IUserQuery userQuery,
                                  IStoreQuery storeQuery,
                                  ApplicationDbContext context)
        {
            _messageCommand = messageCommand;
            _mediator = mediator;
            _logger = logger;
            _notificationCommand = notificationCommand;
            _userQuery = userQuery;
            _storeQuery = storeQuery;
            _context = context;
        }

        public async Task<ServiceResult<ChatDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling send messager");
            if (request.PersonFirstType != Domains.ParticipantType.Buyer)
            {
                _logger.LogError("Role of sender is invalible with this function");
                return ServiceResult<ChatDto>.Error("Vai trò không hợp lệ");
            }
            if (string.IsNullOrWhiteSpace(request.Content))
                return ServiceResult<ChatDto>.Error("Nội dung tin nhắn không được để trống");

            if (request.PersonSecondType == Domains.ParticipantType.Buyer)
            {
                _logger.LogError("Role of reciver is invalible with this function");
                return ServiceResult<ChatDto>.Error("Vai trò không hợp lệ");
            }
            User? buyer = await _userQuery.GetByIdAsync(userId:request.PersonFirstId, cancellationToken);
            if (buyer == null)
            {
                _logger.LogError($"Not found buyer with Id:{request.PersonFirstId}.");
                return ServiceResult<ChatDto>.Error($"Không tìm thấy người mua.");
            }
            var store = await _storeQuery.GetByIdAsync(storeId:request.PersonSecondId,cancellationToken);
            if (store == null)
            {
                _logger.LogError($"Not found store with Id:{request.PersonSecondId}.");
                return ServiceResult<ChatDto>.Error($"Không tìm thấy gian hàng.");
            }
            Notification notification = new Notification(request.PersonSecondId,ReceiverType.Store, request.Content);
            Message message = new Message(ParticipantType.Buyer, request.PersonFirstId, ParticipantType.Store, request.PersonSecondId, request.Content);
            await using var tracsaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                
                await _messageCommand.AddAsync(message);
                await _notificationCommand.addAsync(notification);
                await tracsaction.CommitAsync(cancellationToken);
                return await _mediator.Send(new GetChatQuery());
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error when sending message");
                await tracsaction.RollbackAsync(cancellationToken);
                return ServiceResult<ChatDto>.Error("Xảy ra vấn đề khi gửi tin nhắn");
            }
        }
    }
}
