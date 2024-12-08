namespace Karpinski_XY_Server.Services.Contracts
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration);
        void Remove(string key);
        void RemoveAll(IEnumerable<string> keys);
    }
}
