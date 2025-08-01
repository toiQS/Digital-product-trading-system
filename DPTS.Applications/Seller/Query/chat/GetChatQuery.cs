﻿    using DPTS.Applications.Seller.Dtos.chat;
    using DPTS.Applications.Shareds;
    using MediatR;

    namespace DPTS.Applications.Seller.Query.chat
    {
        public class GetChatQuery : IRequest<ServiceResult<ChatIndexListDto>>
        {
            public string BuyerId { get; set; } = string.Empty;
            public string StoreId { get; set; } = string.Empty;
            public string SellerId { get; set; } = string.Empty;
        }
    }
