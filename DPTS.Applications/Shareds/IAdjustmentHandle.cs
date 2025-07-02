using DPTS.Applications.Buyer.Dtos;
using DPTS.Domains;

namespace DPTS.Applications.Shareds
{
    public interface IAdjustmentHandle
    {
        Task<ServiceResult<MathResultDto>> HandleDiscountAnđPriceForProduct(Product product);
        Task<ServiceResult<MathResultDto>> HandleDiscountForOrderAndPayment(string keyCode, Order order);
    }
}