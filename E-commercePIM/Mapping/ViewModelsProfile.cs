using System.Collections.Generic;
using AutoMapper;
using E_commerce.Library;
using E_commercePIM.Controllers;
using E_commercePIM.ViewModels;

namespace E_commercePIM.Mapping
{
    public class ViewModelsProfile : Profile
    {
        public ViewModelsProfile()
        {
            CreateMap<Product, ProductEditorViewModel>();
            CreateMap<ProductEditorViewModel, Product>();

            CreateMap<Category, CategoryEditorViewModel>();
            CreateMap<CategoryEditorViewModel, Category>();

            CreateMap<Category, CategoryDataViewModel>();
        }
    }
}