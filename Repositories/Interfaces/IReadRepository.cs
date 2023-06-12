namespace Document_Extractor.Repositories.Interfaces;

public interface IReadRepository<T>
{
    public Task<IEnumerable<T>> GetAll();
    public Task<T> GetOne(long itemId);

}