﻿using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.review
{
    public class LikeReviewCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public string ProjectReviewId { get; set; }
    }
}
