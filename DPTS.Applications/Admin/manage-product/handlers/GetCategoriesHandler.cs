﻿using DPTS.Applications.Admin.manage_product.dtos;
using DPTS.Applications.Admin.manage_product.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_product.handlers
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoiesQuery, ServiceResult<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetCategoriesHandler> _logger;

    public GetCategoriesHandler(ICategoryRepository categoryRepository, ILogger<GetCategoriesHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<CategoryDto>> Handle(GetCategoiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling get categories");
        var category = await _categoryRepository.GetsAsync();
        var result = new CategoryDto()
        {
            CategoryIndexDtos = category.Select(x => new CategoryIndexDto()
            {
                Name = x.CategoryName,
                Id = x.CategoryId,
            }).ToList(),
        };
        return ServiceResult<CategoryDto>.Success(result);
    }
}
}
