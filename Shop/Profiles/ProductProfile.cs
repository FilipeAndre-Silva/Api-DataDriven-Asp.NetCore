using AutoMapper;

namespace Shop.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ViewModels.ProductViewModelCreate, Models.Product>();
            CreateMap<ViewModels.ProductViewModelUpdate, Models.Product>();
        }
    }
}