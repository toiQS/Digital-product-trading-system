using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.Buyer.Dtos.product
{
    public class ProductIndexListDto
    {
        public List<ProductIndexDto> ProductIndexList { get; set; } = new List<ProductIndexDto>();
        public List<CategoryIndexDto> Categories { get; set; } = new List<CategoryIndexDto>();
        public List<RateIndexDto> Rates { get; set; } = new List<RateIndexDto> { };
    }
}
