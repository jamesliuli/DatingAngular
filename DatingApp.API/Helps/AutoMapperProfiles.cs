using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helps
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember( dest => dest.PhotoUrl, opt =>
                   opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url));
            CreateMap<User, UserDetailedDto>()
            .ForMember( dest => dest.PhotoUrl, opt =>
                   opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url));
            CreateMap<Photo, PhotosForDetailedDto>();
        }
    }
}