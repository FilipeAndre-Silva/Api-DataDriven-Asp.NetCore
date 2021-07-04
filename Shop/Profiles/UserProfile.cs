using AutoMapper;

namespace Shop.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ViewModels.User.UserViewModelCreate, Models.User>();
            CreateMap<ViewModels.User.UserViewModelUpdate, Models.User>();
            CreateMap<ViewModels.User.UserViewModelAutentication, Models.User>();
        }
    }
}