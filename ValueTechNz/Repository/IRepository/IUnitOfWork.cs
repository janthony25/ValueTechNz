﻿namespace ValueTechNz.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductsRepository Products { get; }
    }
}