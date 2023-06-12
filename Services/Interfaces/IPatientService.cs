
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;

namespace Document_Extractor.Services.Interfaces;

public interface IPatientService
{
    public Task<AppResult<PatientDTO>> GetPatient(long patientId);
    public Task<AppResult<PatientDTO>> GetPatients(bool? isConfirmed);
    public Task<AppResult<PatientDTO>> UploadPatient(UploadRequest formData);
    public Task<AppResult<bool>> ConfirmUploadedPatient(long PatientId, bool status);


}