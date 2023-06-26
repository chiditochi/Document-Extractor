

using Document_Extractor.Models.DB;

namespace Document_Extractor.Repositories.Interfaces;

public interface IAppUserRepository<T> : IRepository<T>
{
    public Task<AppUser> GetByEmail(string email);
}