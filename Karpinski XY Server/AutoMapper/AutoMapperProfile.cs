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
               .ForMember(dest => dest.PaintingPictures, opt => opt.MapFrom(src => src.PaintingPictures))
               .AfterMap((src, dest) => {
                   foreach (var paintingPictureDto in dest.PaintingPictures)
                   {
                       paintingPictureDto.PaintingId = src.Id;
                   }
               });

            CreateMap<PaintingDto, Painting>()
                .ForMember(dest => dest.PaintingPictures, opt => opt.MapFrom(src => src.PaintingPictures))
                .AfterMap((src, dest) => {
                    foreach (var paintingPicture in dest.PaintingPictures)
                    {
                        paintingPicture.PaintingId = src.Id;
                    }
                });

            CreateMap<PaintingPicture, PaintingPictureDto>()
                .ForMember(dest => dest.PaintingId, opt => opt.MapFrom(src => src.PaintingId));

            CreateMap<PaintingPictureDto, PaintingPicture>()
                .ForMember(dest => dest.PaintingId, opt => opt.MapFrom(src => src.PaintingId));

        }
            
        
    }
}
