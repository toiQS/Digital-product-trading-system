using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Buyer.Queries.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.product
{
    public class GetRatingHandler : IRequestHandler<GetRatingQuery, ServiceResult<IEnumerable<RateIndexDto>>>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;

        private readonly ILogger<GetRatingHandler> _logger;

        public GetRatingHandler(IProductReviewRepository productReviewRepository, IProductRepository productRepository, ILogger<GetRatingHandler> logger)
        {
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<RateIndexDto>>> Handle(GetRatingQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get overview rating product");
            var averages = new List<double>();
            var products = (await _productRepository.SearchAsync()).Where(x => x.Status == ProductStatus.Available);
            foreach (var product in products)
            {
                var avgRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                averages.Add(avgRating);
            }

            var result = new List<RateIndexDto>();
            // Thống kê số lượng sản phẩm theo mức đánh giá 1-5 sao
            for (int i = 1; i <= 5; i++)
            {
                var count = averages.Count(x => x >= i && x < i + 1);
                result.Add(new RateIndexDto
                {
                    RatingOverall = i,
                    Count = count
                });
            }
            return ServiceResult<IEnumerable<RateIndexDto>>.Success(result);
        }
    }
}
