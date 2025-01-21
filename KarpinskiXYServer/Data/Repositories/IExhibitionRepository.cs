using Karpinski_XY_Server.Data.Models.Exhibition;

namespace Karpinski_XY_Server.Data.Repositories
{
    public interface IExhibitionRepository
    {
        Task<bool> ExistsAsync(string title);
        Task<Exhibition> FindByIdAsync(Guid id);
        Task<IEnumerable<Exhibition>> GetAllAsync();
        Task AddAsync(Exhibition exhibition);
        Task Delete(Guid id);
    }
}
