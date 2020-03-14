using AutoMapper;
using Webshop.UI.Models;
using Webshop.UI.ViewModels;

namespace Webshop.UI.Mappers
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<Product, ProductEditorViewModel>().ReverseMap();
        }
    }
}