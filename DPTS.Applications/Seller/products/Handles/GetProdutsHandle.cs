using DPTS.Applications.Seller.products.Dtos;
using DPTS.Applications.Seller.products.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using Org.BouncyCastle.Asn1.X500.Style;
using System.Threading.Tasks;

namespace DPTS.Applications.Seller.products.Handles
{
    public class GetProdutsHandle
    {
        private readonly IProductRepository _productRepository;
        //public async Task<ServiceResult<IEnumerable<ProductListItemDto>>> ExcuteAsync(GetProductsItemQuery query)
        //{
        //    var prouduts = await _productRepository.GetsAsync();
        //}
    }
}
