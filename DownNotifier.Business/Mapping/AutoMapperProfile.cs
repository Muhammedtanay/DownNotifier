using AutoMapper;
using DownNotifier.ViewModels;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<TargetApplication, TargetApplicationViewModel>().ReverseMap();
    }
}