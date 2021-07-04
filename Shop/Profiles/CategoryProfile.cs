using AutoMapper;

namespace Shop.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<ViewModels.Category.CategoryViewModelCreate, Models.Category>();
            CreateMap<ViewModels.Category.CategoryViewModelUpdate, Models.Category>();
        }
    }
}