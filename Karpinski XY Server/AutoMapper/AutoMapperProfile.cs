using AutoMapper;
using AutoMapper.Collection;
using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Models;

namespace Karpinski_XY_Server.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<Painting, PaintingDto>();
            CreateMap<PaintingDto, Painting>();

        }
            
        
    }
}
