using Document_Extractor.Models.DB;

namespace Document_Extractor.Repositories.Interfaces;

public interface IAppConstantRepository : IRepository<AppConstant>
{
    public Task<IEnumerable<AppConstant>> GetRequiredProps();
    public Task<IEnumerable<AppConstant>> GetUploadExistProps();
    
}