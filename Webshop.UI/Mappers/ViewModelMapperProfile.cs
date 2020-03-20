using System.Linq;
using AutoMapper;
using E_commerce.Library;
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