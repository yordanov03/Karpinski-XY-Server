using AutoMapper;
using Karpinski_XY_Server.Data.Models.Exhibition;
using Karpinski_XY_Server.Data.Models.Painting;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<Painting, PaintingDto>()
               .ForMember(dest => dest.PaintingImages, opt => opt.MapFrom(src => src.PaintingImages))
               .AfterMap((src, dest) => {
                   foreach (var imageDto in dest.PaintingImages)
                   {
                       imageDto.EntityId = src.Id;
                   }
               });

            CreateMap<PaintingDto, Painting>()
                .ForMember(dest => dest.PaintingImages, opt => opt.MapFrom(src => src.PaintingImages))
                .AfterMap((src, dest) => {
                    foreach (var image in dest.PaintingImages)
                    {
                        image.EntityId = src.Id;
                    }
                });

            CreateMap<PaintingImage, PaintingImageDto>()
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId));

            CreateMap<PaintingImageDto, PaintingImage>()
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId));

            //Exhibition

            CreateMap<Exhibition, ExhibitionDto>()
              .ForMember(dest => dest.ExhibitionImages, opt => opt.MapFrom(src => src.ExhibitionImages))
              .AfterMap((src, dest) => {
                  foreach (var imageDto in dest.ExhibitionImages)
                  {
                      imageDto.EntityId = src.Id;
                  }
              }).ReverseMap();

            //CreateMap<Exhibition, ExhibitionDto>()
            //  .ForMember(dest => dest.ExhibitionImages, opt => opt.MapFrom(src => src.ExhibitionImages))
            //  .AfterMap((src, dest) => {
            //      foreach (var imageDto in dest.ExhibitionImages)
            //      {
            //          imageDto.EntityId = src.Id;
            //      }
            //  });

            CreateMap<ExhibitionImage, ExhibitionImageDto>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
            .ReverseMap();

        }


    }
}
