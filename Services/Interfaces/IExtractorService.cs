
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;

namespace Document_Extractor.Services.Interfaces;

public interface IExtractorService
{
    public Task<AppResult<PatientDTO>> ProcessUpload(UploadRequest formData);
}