using AutoMapper;
using Document_Extractor.Data;
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
    private readonly IPatientTempRepository _patientTempRepository;
    private readonly IHelperService _helperService;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public PatientService(
        ILogger<PatientService> logger,
        IExtractorService extractorService,
        IPatientRepository patientRepository,
        IPatientTempRepository patientTempRepository,
        IHelperService helperService,
        IMapper mapper,
        AppDbContext context
    )
    {
        _logger = logger;
        _extractorService = extractorService;
        _patientRepository = patientRepository;
        _patientTempRepository = patientTempRepository;
        _helperService = helperService;
        _mapper = mapper;
        _context = context;


    }

    // public async Task<AppResult<bool>> ConfirmUploadedPatient(long patientId, bool status)
    // {
    //     var result = new AppResult<bool>();
    //     try
    //     {
    //         result = await _patientTempRepository.ConfirmUpload(patientId, status);
    //     }
    //     catch (Exception ex)
    //     {
    //         await _helperService.CustomLogError(ex, "ConfirmUploadedPatient");
    //         result.Status = false;
    //         result.Message = ex.Message;
    //     }

    //     return result;

    // }

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

    public async Task<AppResult<PatientDTO>> GetPatients(bool? status)
    {
        var result = new AppResult<PatientDTO>();
        try
        {
            var r = await _patientRepository.GetAll();
            // if (r == null || !r.Any()) throw new Exception($"Error fetching Patients");

            //if (status != null) r = r.Where(x => x.Status == status.Value).ToList();
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

    public async Task<AppResult<PatientTempDTO>> UploadPatient(UploadRequest formData)
    {
        var result = new AppResult<PatientTempDTO>();
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


    public async Task<AppResult<bool>> ConfirmUpload(long patientTempId, bool status)
    {
        var result = new AppResult<bool>();
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            /*
                1. if status is true 
                    a. get record from PatientTemp with patientTempId 
                    b. write this to Patient table 
                        - update CreatedAt, UpdatedAt
                        - convert PatientTemp.FileName
                        - convert PatientTemp.TxtFileName 
                        - persist Patient to db 
                        - copy files from TempFolder to configured folders
                2. if status is false
                    - do cleanup 
                3. cleanup 
                    - delete files
                        - PatientTemp.FileName
                        - PatientTemp.TxtFileName
                    - remove entry from db 

            */


            var patientTemp = await _patientTempRepository.GetOne(patientTempId);
            if (patientTemp is null) throw new Exception($"Unable to find PatientTemp record with the PatientTempId = {patientTempId}");

            string fileName = patientTemp.FileName!;
            string txtFileName = patientTemp.TxtFileName!;

            var convertedFiles = await _helperService.GetTempFileConversion(fileName, txtFileName);
            if (status)
            {
                var storePatientResult = await StorePatient(patientTemp, convertedFiles);
                if (!storePatientResult.Status) throw new Exception(storePatientResult.Message);
            }

            var cleanupStatus = await CleanUpTemp(patientTemp);
            await transaction.CommitAsync();

            result.Status = true;
            result.Message = status ? $"Your upload was Successful" : "This upload was Rejected!";
            result.Data.Add(status);
        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<AppResult<bool>> CleanUpTemp(PatientTemp patientTemp)
    {
        var result = new AppResult<bool>();
        try
        {
            /*
                1. cleanup 
                    - delete files
                        - PatientTemp.FileName
                        - PatientTemp.TxtFileName
                    - remove entry from db 

            */

            string fileName = patientTemp.FileName!;
            string txtFileName = patientTemp.TxtFileName!;

            //var convertedFiles = await _helperService.GetTempFileConversion(fileName, txtFileName);

            var patientTempResult = await _patientTempRepository.Delete(patientTemp.PatientTempId);
            if (patientTempResult)
            {
                var cleanUpResult = await _helperService.CleanupTempUploadFiles(fileName, txtFileName);
            }

            result.Status = true;
            result.Message = "Cleanup was Successful";
            result.Data.Add(true);

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<AppResult<bool>> StorePatient(PatientTemp patientTemp, List<string> convertedFiles)
    {
        var result = new AppResult<bool>();
        try
        {
            /*
                1. if status is true 
                    a. get record from PatientTemp with patientTempId 
                    b. write this to Patient table 
                        - update CreatedAt, UpdatedAt
                        - convert PatientTemp.FileName
                        - convert PatientTemp.TxtFileName 
                        - persist Patient to db 
                        - copy files from TempFolder to configured folders

            */
            string fileName = patientTemp.FileName!;
            string txtFileName = patientTemp.TxtFileName!;
            //var convertedFiles = await _helperService.GetTempFileConversion(fileName, txtFileName);

            var patient = _mapper.Map<Patient>(patientTemp);
            patient.CreatedAt = DateTime.Now;
            patient.UpdatedAt = DateTime.Now;
            patient.FileName = convertedFiles.First();
            patient.TxtFileName = convertedFiles.Last();

            var dbPatient = await _patientRepository.Create(patient);
            if (dbPatient is null) throw new Exception($"Error creating Patient with PatientTempId = {patientTemp.PatientTempId}");

            var copyResult = await _helperService.CopyFilesToConfiguredFolders(fileName, txtFileName, convertedFiles);
            if (!copyResult) throw new Exception($"Error copying Files to Configured Folders for  with PatientTempId = {patientTemp.PatientTempId}");


            result.Status = true;
            result.Data.Add(true);
            result.Message = $"Uploaded TempData was Successfully copied!";

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }





}