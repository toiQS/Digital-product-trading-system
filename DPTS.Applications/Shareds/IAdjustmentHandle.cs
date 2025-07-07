using DPTS.Applications.Buyer.Dtos.shared;
using DPTS.Domains;

namespace DPTS.Applications.Shareds
{
    public interface IAdjustmentHandle
    {
        Task<ServiceResult<MathResultDto>> HandleDiscountAndPriceForProduct(Product product);
        Task<ServiceResult<MathResultDto>> HandleDiscountForOrderAndPayment(string keyCode = null!, Order order= default!);
        Task<ServiceResult<MathResultDto>> HandleTaxForSeller(decimal finalPrice);
        Task<ServiceResult<MathResultDto>> HandlePlatformFeeForSeller(decimal finalPrice);
    }
}