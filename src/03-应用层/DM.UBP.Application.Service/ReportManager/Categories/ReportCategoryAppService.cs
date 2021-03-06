﻿//------------------------------------------------------------
// All Rights Reserved , Copyright (C)  
// 版本：1.0
/// <author>
///		<name></name>
///		<date>0001/1/1 0:00:00</date>
/// </author>
//------------------------------------------------------------

using System.Threading.Tasks;
using System.Collections.Generic;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Authorization;
using Abp.Application.Services.Dto;
using DM.UBP.Domain.Entity.ReportManager;
using DM.UBP.Domain.Service.ReportManager.Categories;
using DM.UBP.Domain.Service.ReportManager;
using DM.UBP.Application.Dto.ReportManager.Categories;
using System.Linq;
using DM.UBP.Dto;
using System.Linq.Dynamic;

namespace DM.UBP.Application.Service.ReportManager.Categories
{
    /// <summary>
    /// 报表分类的Application.Service
    /// <summary>
    [AbpAuthorize(AppPermissions_ReportManager.Pages_ReportManager_Categories, AppPermissions_ReportManager.Pages_Reports)]
    public class ReportCategoryAppService : IReportCategoryAppService
    {
        private readonly IReportCategoryManager _CategoryManager;
        public ReportCategoryAppService(
           IReportCategoryManager categorymanager
           )
        {
            _CategoryManager = categorymanager;
        }

        public async Task<List<ComboboxItemDto>> GetCategoriesToItem(long selectValue)
        {
            var entities = await _CategoryManager.GetAllCategoriesAsync();
            var items = entities.Select(c => new ComboboxItemDto(c.Id.ToString(), c.CategoryName) { IsSelected = c.Id == selectValue }).ToList();

            return items;
        }

        public async Task<PagedResultDto<ReportCategoryOutputDto>> GetCategories()
        {
            var entities = await _CategoryManager.GetAllCategoriesAsync();
            var listDto = entities.MapTo<List<ReportCategoryOutputDto>>();

            return new PagedResultDto<ReportCategoryOutputDto>(
            listDto.Count,
            listDto
            );

        }
        public async Task<PagedResultDto<ReportCategoryOutputDto>> GetCategories(PagedAndSortedInputDto input)
        {
            var entities = await _CategoryManager.GetAllCategoriesAsync();

            if (string.IsNullOrEmpty(input.Sorting))
                input.Sorting = "Id";

            var orderEntities = await Task.FromResult(entities.OrderBy(input.Sorting));

            var pageEntities = await Task.FromResult(orderEntities.Skip(input.SkipCount).Take(input.MaxResultCount));

            var listDto = pageEntities.MapTo<List<ReportCategoryOutputDto>>();

            return new PagedResultDto<ReportCategoryOutputDto>(
            entities.Count,
            listDto
            );
        }
        public async Task<ReportCategoryOutputDto> GetCategoryById(long id)
        {
            var entity = await _CategoryManager.GetCategoryByIdAsync(id);
            return entity.MapTo<ReportCategoryOutputDto>();
        }
        [AbpAuthorize(AppPermissions_ReportManager.Pages_ReportManager_Categories_Create)]
        public async Task<bool> CreateCategory(ReportCategoryInputDto input)
        {
            input.ParentId = 0;
            input.Code = "0000";
            var entity = input.MapTo<ReportCategory>();
            return await _CategoryManager.CreateCategoryAsync(entity);
        }
        [AbpAuthorize(AppPermissions_ReportManager.Pages_ReportManager_Categories_Edit)]
        public async Task<bool> UpdateCategory(ReportCategoryInputDto input)
        {
            input.ParentId = 0;
            input.Code = "0000";
            var entity = await _CategoryManager.GetCategoryByIdAsync(input.Id);
            input.MapTo(entity);
            return await _CategoryManager.UpdateCategoryAsync(entity);
        }
        [AbpAuthorize(AppPermissions_ReportManager.Pages_ReportManager_Categories_Delete)]
        public async Task DeleteCategory(EntityDto input)
        {
            var entity = await _CategoryManager.GetCategoryByIdAsync(input.Id);
            await _CategoryManager.DeleteCategoryAsync(entity);
        }
    }
}
