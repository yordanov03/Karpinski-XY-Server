using Karpinski_XY_Server.Dtos.BaseDto;

namespace Karpinski_XY_Server.Services.FileServices
{
    public interface IImagePathService<T> where T : ImageBaseDto
    {
        string ConstructPathForConversionTo64Base(T imageDto);
        string ConstructPathForDatabase(T imageDto);
        string GetBaseUrlFromLaunchSettings();
    }
}
