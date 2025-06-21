using DPTS.Applications.NoDistinctionOfRoles.homePages.Dtos;
using DPTS.Applications.Shareds;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.NoDistinctionOfRoles.homePages.Queries
{
    public class GetTopSellingProductsQuery : IRequest<ServiceResult<IEnumerable<TopSellingProductDto>>>
    {
    }
}
