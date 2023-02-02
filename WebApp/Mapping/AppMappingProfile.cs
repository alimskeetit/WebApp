using AutoMapper;
using WebApp.AccountManager;
using WebApp.ViewModels;

namespace WebApp.Mapping
{
    public class AppMappingProfile: Profile
    {
        public AppMappingProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(user => user.UserName, opt => opt.MapFrom(regModel => regModel.EmailReg))
                .ForMember(user => user.Email, opt => opt.MapFrom(regModel => regModel.EmailReg))
                .ForMember(user => user.BirthDate, opt => opt.MapFrom(regModel => new DateTime(regModel.Year, regModel.Month, regModel.Date)));
            CreateMap<User, UserViewModel>();
            CreateMap<LoginViewModel, User>();

            CreateMap<User, UserEditViewModel>()
                .ForMember(userEdit => userEdit.UserId, opt => opt.MapFrom(user => user.Id))
                .ForMember(userEdit => userEdit.Image, opt => opt.MapFrom(user => user.Image));
            CreateMap<User, UserWithFriendExt>();
        }
    }
}
