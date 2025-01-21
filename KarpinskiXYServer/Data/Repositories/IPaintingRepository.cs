using Karpinski_XY_Server.Data.Models.Painting;

namespace Karpinski_XY_Server.Data.Repositories
{
    public interface IPaintingRepository
    {
        Task<bool> ExistsAsync(string name);
        Task<Painting> FindByIdAsync(Guid id);
        Task AddAsync(Painting painting);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Painting>> GetAllToSellAsync();
        Task<IEnumerable<Painting>> GetAvailableAsync();
        Task<IEnumerable<Painting>> GetPortfolioAsync();
        Task<IEnumerable<Painting>> GetOnFocusAsync();
    }

}
