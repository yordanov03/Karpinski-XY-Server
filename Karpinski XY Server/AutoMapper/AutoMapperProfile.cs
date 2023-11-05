using AutoMapper;
using Karpinski_XY_Server.Data.Models;
using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Models;

namespace Karpinski_XY_Server.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            //CreateMap<Painting, PaintingDto>();
            //CreateMap<PaintingDto, Painting>();
            //CreateMap<PaintingPicture, PaintingPictureDto>();
            //CreateMap<PaintingPictureDto, PaintingPicture>();
            CreateMap<Painting, PaintingDto>()
               .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
               .AfterMap((src, dest) => {
                   foreach (var imageDto in dest.Images)
                   {
                       imageDto.PaintingId = src.Id;
                   }
               });

            CreateMap<PaintingDto, Painting>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .AfterMap((src, dest) => {
                    foreach (var image in dest.Images)
                    {
                        image.PaintingId = src.Id;
                    }
                });

            CreateMap<Image, ImageDto>()
                .ForMember(dest => dest.PaintingId, opt => opt.MapFrom(src => src.PaintingId));

            CreateMap<ImageDto, Image>()
                .ForMember(dest => dest.PaintingId, opt => opt.MapFrom(src => src.PaintingId));

        }
            
        
    }
}
