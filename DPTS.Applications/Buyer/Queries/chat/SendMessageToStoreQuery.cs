﻿using DPTS.Applications.Buyer.Dtos.chat;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.chat
{
    public class SendMessageToStoreQuery: IRequest<ServiceResult<ChatIndexListDto>>
    {
        public string StoreId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;
    }
}
