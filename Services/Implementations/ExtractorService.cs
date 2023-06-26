using System.Globalization;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AutoMapper;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.DTOs;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Document_Extractor.Services.Implementation;
using Document_Extractor.Services.Interfaces;

namespace Document_Extractor.Services.Implementations;


public class ExtractorService : IExtractorService
{
    private readonly ILogger<ExtractorService> _logger;
    private readonly IConfiguration _config;
    private readonly IHelperService _helperService;
    private readonly IAppConstantRepository _appConstantRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _wenv;

    public ExtractorService(
        ILogger<ExtractorService> logger,
        IConfiguration config,
        IHelperService helperService,
        IAppConstantRepository appConstantRepository,
        ITeamRepository teamRepository,
        IPatientRepository patientRepository,
        IMapper mapper,
        IWebHostEnvironment wenv
    )
    {
        _logger = logger;
        _config = config;
        _helperService = helperService;
        _appConstantRepository = appConstantRepository;
        _teamRepository = teamRepository;
        _patientRepository = patientRepository;
        _mapper = mapper;
        _wenv = wenv;


    }

    public async Task<AppResult<PatientDTO>> ProcessUpload(UploadRequest formData)
    {
        var result = new AppResult<PatientDTO>();

        var filePath = string.Empty;
        var textPath = string.Empty;
        try
        {
            /*
                1. get extractor factory based on file extension 
                2. call extractor to get extract file txt
                3. get file properties 
                4. validate 
                    - Required fields 
                    - Unique request 
                5. persist data 
                6. return data model for user confirmation 
                7. modify persisted data based on users response 
                8. optionally: 
                    - write files to configured directories 
            */

            var fileExtension = Path.GetExtension(formData?.Doc?.FileName);
            var fileStorageResult = await StoreFile(formData?.Doc!);
            if (!fileStorageResult.Status) throw new Exception(fileStorageResult.Message);

            filePath = fileStorageResult.Data.First();
            var extractFileResult = await ExtractFile(filePath);
            if (!extractFileResult.Status) throw new Exception(extractFileResult.Message);

            var extractFile = extractFileResult.Data.First();
            var fileModelResult = await GetFileModel(extractFile);
            if (!fileModelResult.Status) throw new Exception(fileModelResult.Message);
            textPath = fileModelResult.Data.First().StorageName;

            //append Team Details to txtfile 
            var appendResult = await AppendTeamDetailsToTxtData(textPath, formData!.TeamId);
            //copy txt file to Uploads folder 
            var copyFileResult = await CopyTxtFileToUploadsFolder(textPath);

            var extractModel = fileModelResult.Data.First();
            var validationResult = await ValidateFileModel(extractModel);
            if (!validationResult.Status) throw new Exception(validationResult.Message);

            //persist data 
            var patientDTO = await StoreUploadPatient(formData!.TeamId, filePath, textPath, extractModel);

            var viewDateTimeFormat = _config.GetSection("App:ViewDateTimeFormat").Value;
            patientDTO.DateTimeString = patientDTO.DateTime.ToString(viewDateTimeFormat);
            patientDTO.PatientDOBString = patientDTO.PatientDOB.ToString(viewDateTimeFormat);

            result.Status = true;
            result.Data.Add(patientDTO);


        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "ProcessUpload");
            await _helperService.DeleteFiles(new string[] { filePath, textPath });
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }

    private async Task<PatientDTO> StoreUploadPatient(long teamId, string filePath, string textPath, ExtractResult extractModel)
    {
        ExtractDTO model = extractModel.Data;
        var patient = _mapper.Map<ExtractDTO, Patient>(model);
        patient.CreatedAt = DateTime.Now;
        patient.UpdatedAt = DateTime.Now;
        patient.TeamId = teamId;
        patient.FileName = filePath;
        patient.TxtFileName = textPath;
        patient.IsUploadComfirmed = false;
        patient.Status = false;

        var dbPatient = await _patientRepository.Create(patient);
        if (dbPatient == null) throw new Exception($"Error creating Patient");
        var resultDTO = _mapper.Map<Patient, PatientDTO>(dbPatient);
        return resultDTO;
    }

    // private static string StorageName { get; set; } = string.Empty;
    // private static string StorageName2 { get; set; } = string.Empty;
    // private static StringBuilder DataBuffer { get; set; } = new StringBuilder();



    private async Task<AppResult<string>> StoreFile(IFormFile doc)
    {
        var result = new AppResult<string>();
        try
        {
            var storageLocation = _config.GetSection("App:Uploads:UserUploads").Value;
            storageLocation = string.Join(Path.DirectorySeparatorChar, storageLocation.Split("/"));
            if (string.IsNullOrEmpty(storageLocation)) throw new Exception($"No Upload Location configured!");
            var fileNameWithPath = Path.Combine(_wenv.WebRootPath, storageLocation, DateTimeOffset.Now.ToUnixTimeSeconds() + "_" + doc.FileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                doc.CopyTo(stream);
            }

            result.Status = true;
            result.Data.Add(fileNameWithPath);

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "StoreFile");
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }
    private async Task<AppResult<ExtractResult>> ExtractFile(string filePath)
    {
        var result = new AppResult<ExtractResult>();
        try
        {
            ExtractResult r = new ExtractResult();
            IExtractor? extractor = GetExtractorFactory(filePath);
            if (extractor == null) throw new Exception($"Error fetching Extractor instance");

            var targetFilePath = _config.GetSection("App:Uploads:txtUploads").Value;
            targetFilePath = string.Join(Path.DirectorySeparatorChar, targetFilePath.Split("/"));
            if (string.IsNullOrEmpty(targetFilePath)) throw new Exception();

            targetFilePath = Path.Combine(_wenv.WebRootPath, targetFilePath);
            var extractFileResult = await extractor!.ReadFile(filePath, targetFilePath);
            if (!extractFileResult.Status) throw new Exception(extractFileResult.Message);

            r.StorageName = extractFileResult.Data.First();
            result.Status = true;
            result.Data.Add(r);

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "ExtractFile");
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }
    private IExtractor? GetExtractorFactory(string filePath)
    {
        IExtractor result = null;
        var ext = Path.GetExtension(filePath);
        if (ext == ".docx") result = new WordExtractor();
        if (ext == ".pdf") result = new PdfExtractor();

        return result;
    }

    private async Task<AppResult<ExtractResult>> GetFileModel(ExtractResult extract)
    {
        var result = new AppResult<ExtractResult>();
        try
        {
            //    var result = new ExtractResult();
            //     result.StorageName = StorageName;
            //     result.StorageName2 = StorageName2;
            //     result.Data = new ExtractDTO();

            var props = GetEntryNames();
            var propsAndFormat = PropsAndFormat();

            string[] content = File.ReadAllLines(extract.StorageName) ?? throw new Exception("Error reading {StorageName}");
            //find line where text = Appendix – Please ignore. This section must not be edited.
            var targetString = "Appendix – Please ignore. This section must not be edited.";

            var targetIndex = -1;

            for (var i = 0; i < content.Length; i++)
            {
                if (content[i].Trim().StartsWith(targetString))
                {
                    targetIndex = i;
                    break;
                }
            }

            string[] truncatedString = content.AsEnumerable()
                                        .Select((x, i) => new { Text = x, Index = i })
                                        .Where(x => x.Index > targetIndex && !string.IsNullOrEmpty(x.Text.Trim()))
                                        .Select(x => x.Text)
                                        .ToArray();

            ExtractDTO rd = new ExtractDTO();

            foreach (var value in truncatedString)
            {
                foreach (var prop in props)
                {
                    if (value.StartsWith(prop) && value.Split(":").First() == prop)
                    {
                        var itemValue = value.Replace(prop + ":", "").Trim();
                        var formatter = string.Empty;

                        Type type = rd.GetType();
                        PropertyInfo propItem = type.GetProperty(prop)!;

                        if (propsAndFormat.ContainsKey(prop))
                        {
                            formatter = propsAndFormat[prop];
                            DateTime? formattedValue = GetDateType(itemValue, formatter);

                            if (formattedValue == null) propItem?.SetValue(rd, null, null);
                            else propItem?.SetValue(rd, formattedValue.Value, null);

                        }
                        else
                        {
                            propItem?.SetValue(rd, itemValue, null);
                        }

                    }
                }

            }

            extract.Data = rd;
            result.Data.Add(extract);
            result.Status = true;

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "GetFileModel");
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }

    private async Task<bool> AppendTeamDetailsToTxtData(string textPath, long teamId)
    {
        bool result = false;
        try
        {
            var team = await _teamRepository.GetOne(teamId);
            if (team is null) throw new Exception($"Unable to fetch Team with Id ={teamId}");

            if (!File.Exists(textPath)) throw new Exception($"{textPath} does not exist");

            var content = new string[] { $"TeamCode: {team.Code}", $"TeamDescription: {team.CodeDescription}" };
            await File.AppendAllLinesAsync(textPath, content);

            result = true;
        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "AppendTeamDetailsToTxtData");
            result = false;
        }

        return result;
    }
    private async Task<bool> CopyTxtFileToUploadsFolder(string textPath)
    {
        bool result = false;
        try
        {
            /*
                C:\Projects\UploadConverter\wwwroot\Uploads\txtUploads\1687716067_James-Brown.txt
                C:\Projects\UploadConverter\wwwroot\Uploads\UserUploads\1687716067_James-Brown.docx
            */
            var fileName = textPath.Split(Path.DirectorySeparatorChar).Last();

            var storageLocation = _config.GetSection("App:Uploads:UserUploads").Value;
            storageLocation = string.Join(Path.DirectorySeparatorChar, storageLocation.Split("/"));
            if (string.IsNullOrEmpty(storageLocation)) throw new Exception($"No Upload Location configured!");
            var destinationPath = Path.Combine(_wenv.WebRootPath, storageLocation, fileName);


            File.Copy(textPath, destinationPath);

            result = true;
        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "CopyTxtFileToUploadsFolder");
            result = false;
        }

        return result;
    }

    private static Dictionary<string, string> PropsAndFormat()
    {
        return new Dictionary<string, string>()
        {
            { "DateTime", "dd/MM/yyyy HH:mm" },
            { "PatientDOB", "dd MMM yyyy" }
        };
    }

    private static List<string> GetEntryNames()
    {
        var entries = new List<string>()
            {
                "DateTime",
                "OperatorFirstname",
                "OperatorMiddle",
                "OperatorSurname",
                "PatientNHS",
                "PatientTitle",
                "PatientFirstname",
                "PatientMiddle",
                "PatientSurname",
                "PatientDOB",
                "PatientSex",
                "PatientSexCode",
                "PatientHousename",
                "PatientAddress1",
                "PatientAddress2",
                "PatientAddress3",
                "PatientAddress4",
                "PatientPostcode",
                "PatientPhoneno",
                "PatientReligion",
                "PatientEthnicity",
                "PatientPractice",
                "PatientPracticeAddress",
                "PatientPracticeCode",
                "PatientGPTitle",
                "PatientGPFirstName",
                "PatientGPSurname",
                "PatientGPCode"
            };

        return entries;

    }

    private static DateTime? GetDateType(string input, string format)
    {
        DateTime? result = null;
        try
        {
            result = DateTime.ParseExact(input, format, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetDateType: input = {input}, format = {format} {ex.Message}");
        }

        return result;
    }


    private async Task<AppResult<bool>> ValidateFileModel(ExtractResult extractResult)
    {
        var result = new AppResult<bool>();
        try
        {

            await RequiredPropsValidation(extractResult);
            await UploadExistValidation(extractResult);

            result.Data.Add(true);
            result.Status = true;

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "ValidateFileModel");
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }

    private async Task RequiredPropsValidation(ExtractResult extractResult)
    {
        /*
            1. get required props from db 
            2. check model for each 
            3. throw error if any is missing 
        */

        var requiredPropsResult = await _appConstantRepository.GetRequiredProps();
        if (requiredPropsResult == null) throw new Exception($"Error fetching {SharedConstants.RequiredProps}");

        List<string> requiredProps = requiredPropsResult.Where(x => !string.IsNullOrEmpty(x.LabelValue)).Select(x => x.LabelValue!).ToList<string>();

        ExtractDTO model = extractResult.Data;
        Type type = model.GetType();
        var missingList = new List<string>();
        foreach (var prop in requiredProps)
        {
            PropertyInfo propItem = type.GetProperty(prop)!;
            var propValue = propItem.GetValue(model, null);
            if (propValue == null || string.IsNullOrEmpty(propValue?.ToString())) missingList.Add(prop);
        }


        if (missingList.Count > 0)
        {
            var errorMessage = missingList.Count == 1 ? $"{string.Join(", ", missingList)} is Required!" : $"Required fields missing are {string.Join(", ", missingList)}";
            throw new Exception(errorMessage);
        }


    }

    private async Task UploadExistValidation(ExtractResult extractResult)
    {
        /*
         1. get required props from db 
         2. check model for each 
         3. throw error if any is missing 
     */

        var uploadExistPropsResult = await _appConstantRepository.GetUploadExistProps();
        if (uploadExistPropsResult != null && uploadExistPropsResult?.Count() > 0)
        {
            List<string> uploadExistProps = uploadExistPropsResult.Where(x => !string.IsNullOrEmpty(x.LabelValue)).Select(x => x.LabelValue!).ToList<string>();



            var propsAndFormat = PropsAndFormat();
            var modelPropsDict = new Dictionary<string, string>();

            ExtractDTO model = extractResult.Data;
            Type type = model.GetType();
            foreach (string prop in uploadExistProps)
            {
                PropertyInfo propItem = type.GetProperty(prop)!;
                var propValue = propItem.GetValue(model, null);
                if (propValue != null || !string.IsNullOrEmpty(propValue?.ToString())) modelPropsDict[prop] = propValue.ToString()!;
            }


            var uploadExistResult = await _patientRepository.DoesUploadExist(modelPropsDict, propsAndFormat);
            if (!uploadExistResult.Status) throw new Exception(uploadExistResult.Message);
            if (uploadExistResult.Data.First()) throw new Exception($"This form has been uploaded before!");

        }
    }




}