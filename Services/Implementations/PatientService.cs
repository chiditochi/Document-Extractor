using AutoMapper;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Document_Extractor.Services.Interfaces;

namespace Document_Extractor.Services.Implementations;


public class PatientService : IPatientService
{
    private readonly ILogger<PatientService> _logger;
    private readonly IExtractorService _extractorService;
    private readonly IPatientRepository _patientRepository;
    private readonly IHelperService _helperService;
    private readonly IMapper _mapper;

    public PatientService(
        ILogger<PatientService> logger,
        IExtractorService extractorService,
        IPatientRepository patientRepository,
        IHelperService helperService,
        IMapper mapper
    )
    {
        _logger = logger;
        _extractorService = extractorService;
        _patientRepository = patientRepository;
        _helperService = helperService;
        _mapper = mapper;


    }

    public async Task<AppResult<bool>> ConfirmUploadedPatient(long patientId, bool status)
    {
        var result = new AppResult<bool>();
        try
        {
            result = await _patientRepository.ConfirmUpload(patientId, status);
        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "ConfirmUploadedPatient");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }

    public async Task<AppResult<PatientDTO>> GetPatient(long patientId)
    {
        var result = new AppResult<PatientDTO>();
        try
        {
            var r = await _patientRepository.GetOne(patientId);
            if (r == null) throw new Exception($"Error fetching Patient with id = {patientId}");

            var dto = _mapper.Map<PatientDTO>(r);
            result.Status = true;
            result.Data.Add(dto);

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "GetPatient");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public async Task<AppResult<PatientDTO>> GetPatients(bool? isUploadComfirmed)
    {
        var result = new AppResult<PatientDTO>();
        try
        {
            var r = await _patientRepository.GetAll();
            // if (r == null || !r.Any()) throw new Exception($"Error fetching Patients");

            if (isUploadComfirmed != null) r = r.Where(x => x.IsUploadComfirmed == isUploadComfirmed.Value).ToList();
            var dtos = _mapper.Map<IEnumerable<PatientDTO>>(r);
            result.Data.AddRange(dtos);

            result.Status = true;

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "GetPatients");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public async Task<AppResult<PatientDTO>> UploadPatient(UploadRequest formData)
    {
        var result = new AppResult<PatientDTO>();
        try
        {
            result = await _extractorService.ProcessUpload(formData);
        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "UploadPatient");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }


}