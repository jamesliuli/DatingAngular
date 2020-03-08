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
                   opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url))
            .ForMember( dest => dest.Age, opt =>
                   opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserDetailedDto>()
            .ForMember( dest => dest.PhotoUrl, opt =>
                   opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url))
            .ForMember( dest => dest.Age, opt =>
                   opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
                   
            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<UserForRegisterDto, User>();

            CreateMap<Message, MessageForReturnDto>()
            .ForMember( dest => dest.SenderKnownAs, opt =>
                    opt.MapFrom( src => src.Sender.KnownAs))
            .ForMember( dest => dest.SenderPhotoUrl, opt =>
                   opt.MapFrom( src => src.Sender.Photos.FirstOrDefault( p => p.IsMain).Url))
            .ForMember( dest => dest.RecipientKnownAs, opt =>
                    opt.MapFrom( src => src.Recipient.KnownAs))
            .ForMember( dest => dest.RecipientPhotoUrl, opt =>
                    opt.MapFrom( src => src.Recipient.Photos.FirstOrDefault( p => p.IsMain).Url));

            CreateMap<MessageForCreationDto, Message>();

        }
    }
}