﻿using ValueTechNz.Models.Dto;

namespace ValueTechNz.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<List<GetProductsDto>> GetAllProductsAsync();
    }
}