namespace Document_Extractor.Repositories.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetOne(long itemId);
    Task<T> Create(T item);
    Task<IEnumerable<T>> CreateMany(IEnumerable<T> items);
    Task<bool> Update(T item, long itemId);
    Task<bool> Delete(long itemId);
}