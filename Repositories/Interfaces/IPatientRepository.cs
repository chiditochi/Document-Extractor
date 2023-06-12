using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;

namespace Document_Extractor.Repositories.Interfaces;

public interface IPatientRepository: IRepository<Patient>
{
    public Task<AppResult<bool>> ConfirmUpload(long patientId, bool status);
    public Task<AppResult<bool>> DoesUploadExist(Dictionary<string, string> props, Dictionary<string, string> propsAndFormat);
}