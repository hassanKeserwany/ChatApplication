using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                //to add photoUrl from src (Photos), photoUrl is the photo that has (IsMain =true)
                .ForMember(dest=>dest.PhotoUrl,
                          option=>option.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(dest =>dest.Age , option =>option.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();



            CreateMap<RegisterDto, AppUser>()
            .ForMember(dest => dest.DateOfBirth,
                       opt => opt.MapFrom(src => src.DateOfBirth.ToDateTime(new TimeOnly(0, 0))));
        }

    }
    }
