using DPTS.Applications.case_buyer.homepage.dtos;
using DPTS.Applications.shareds;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.case_buyer.homepage.models
{
    internal class GetTopSaleProductIndexQuery : IRequest<ServiceResult<IEnumerable<TopSateProductIndexDto>>>
    {
    }
}
