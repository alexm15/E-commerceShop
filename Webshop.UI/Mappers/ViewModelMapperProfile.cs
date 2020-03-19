using System.Linq;
using AutoMapper;
using Webshop.UI.App_Data;
using Webshop.UI.Models;
using Webshop.UI.ViewModels;

namespace Webshop.UI.Mappers
{
    public class ViewModelMapperProfile : Profile
    {
        public ViewModelMapperProfile()
        {
            CreateMap<Product, ProductEditorViewModel>();
            
            
            CreateMap<Category, AssignedCategoryData>();
        }
    }
}