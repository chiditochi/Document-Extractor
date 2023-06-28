

using Document_Extractor.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Document_Extractor.Services.Implementation;

public class HelperService : IHelperService
{
    private readonly ILogger<HelperService> _logger;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _wenv;

    public HelperService(
        ILogger<HelperService> logger,
        IConfiguration config,
        IWebHostEnvironment wenv
    )
    {
        _logger = logger;
        _config = config;
        _wenv = wenv;
    }
    public Task CustomLogError(Exception e, string action)
    {
        string message = e.Message;
        if (e.InnerException != null && e.InnerException.Message != null)
        {
            message = e.InnerException.Message;
        }
        if (!string.IsNullOrEmpty(e.StackTrace))
        {
            message += $"\n {e.StackTrace}";
        }
        _logger.LogError($"{action}:: {message}");
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteFile(string filePath)
    {
        bool result;
        try
        {
            var pathAndFileName = filePath;
            if (File.Exists(pathAndFileName))
            {
                File.Delete(pathAndFileName);
            }
            result = true;
        }
        catch (Exception ex)
        {
            await CustomLogError(ex, $"DeleteFile");
            result = false;
        }
        return result;
    }
    public async Task<bool> DeleteFiles(string[] filePaths)
    {
        bool result;
        try
        {
            foreach (var item in filePaths)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    await DeleteFile(item);
                }
            }

            result = true;
        }
        catch (Exception ex)
        {
            await CustomLogError(ex, $"DeleteFiles");
            result = false;
        }
        return result;
    }

    private static string GetSqlString<T>(List<T> items)
    {
        var result = string.Empty;
        var list = new List<object>();
        if (typeof(T) == typeof(string))
        {
            foreach (var item in items)
            {
                list.Add($"'{item}'");
            }
        }
        else
        {
            foreach (var item in items)
            {
                list.Add($"{item}");
            }
        }

        return $"({string.Join(", ", list)})";
    }


    public List<string> GetModelStateErrors(ModelStateDictionary modelState)
    {
        return modelState.Values.SelectMany(v => v.Errors)
                                    .Select(v => v.ErrorMessage + " " + v.Exception)
                                    .ToList();
    }

    public async Task<bool> CreateFolder(string filePath)
    {
        bool result = false;
        try
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                _logger.LogWarning($"{filePath} Root folder was created!");
            }
            result = true;
        }
        catch (Exception e)
        {
            await CustomLogError(e, $"CreateFolderAndDeleteFile");
            result = false;
        }
        return result;
    }

    public async Task<bool> CreateUploadFolders()
    {
        bool result = false;
        try
        {

            var webroot = _wenv.WebRootPath;

            var uploadPath = _config.GetSection("App:Uploads:UserUploads").Value;
            uploadPath = GetFormattedPath(uploadPath);
            uploadPath = Path.Join(webroot, uploadPath);

            var txtUploadPath = _config.GetSection("App:Uploads:txtUploads").Value;
            txtUploadPath = GetFormattedPath(txtUploadPath);
            txtUploadPath = Path.Join(webroot, txtUploadPath);

            var tempUploadsPath = _config.GetSection("App:TempUploads").Value;
            tempUploadsPath = GetFormattedPath(tempUploadsPath);
            tempUploadsPath = Path.Join(webroot, tempUploadsPath);

            await CreateFolder(uploadPath);
            await CreateFolder(txtUploadPath);
            await CreateFolder(tempUploadsPath);

            result = true;
        }
        catch (Exception e)
        {
            await CustomLogError(e, $"CreateUploadFolders");
            result = false;
        }
        return result;
    }
    public async Task<bool> CleanupTempUploadFiles(string fileName, string txtFileName)
    {
        bool result = false;
        try
        {

            var files = new string[] { fileName, txtFileName };

            foreach (var itemFile in files)
            {
                if (File.Exists(itemFile))
                {
                    File.Delete(itemFile);
                }
            }

            result = true;
        }
        catch (Exception e)
        {
            await CustomLogError(e, $"CleanupTempUploadFiles");
            result = false;
        }
        return result;
    }

    public async Task<bool> CopyFilesToConfiguredFolders(string fileName, string txtFileName, List<string> convertedFileNames)
    {
        bool result = false;
        try
        {

            // var f = await GetFileNamesFromTempPath(fileName, txtFileName);
            // if (!f.Any()) throw new Exception($"Error converting TempFile names to Configured Uploads directories");
            // if (f.Count != 2) throw new Exception($"Error converting TempFile names, {f.Count} filename were returned");

            var uploadFileName = fileName.Split(Path.DirectorySeparatorChar).Last();
            var extractedFileName = txtFileName.Split(Path.DirectorySeparatorChar).Last();

            var targetFileNamePath = convertedFileNames.First();
            var targetFiles2 = targetFileNamePath.Split(Path.DirectorySeparatorChar);
            var t2 = targetFiles2.Last();
            var t2FileName = $"{t2.Split(".")[0]}.txt";
            targetFiles2[targetFiles2.Length - 1] = t2FileName;
            var targetFileNamePath2 = string.Join(Path.DirectorySeparatorChar, targetFiles2);

            //copy to uploads folder 
            File.Copy(fileName, targetFileNamePath);
            File.Copy(txtFileName, targetFileNamePath2);
            _logger.LogInformation($"Done copying {fileName} and {txtFileName} to {targetFileNamePath.Replace(uploadFileName, "")}");



            var targetTxtFileNamePath = convertedFileNames.Last();
            //copy to txt upload folder 
            File.Copy(txtFileName, targetTxtFileNamePath);
            _logger.LogInformation($"Done copying {txtFileName} to {targetTxtFileNamePath.Replace(extractedFileName, "")}");

            result = true;
        }
        catch (Exception e)
        {
            await CustomLogError(e, $"CopyFilesToConfiguredFolders");
            result = false;
        }
        return result;
    }

    public async Task<List<string>> GetTempFileConversion(string fileName, string txtFileName)
    {
        var result = new List<string>();
        try
        {

            var webroot = _wenv.WebRootPath;

            var uploadPath = _config.GetSection("App:Uploads:UserUploads").Value;
            uploadPath = GetFormattedPath(uploadPath);
            uploadPath = Path.Join(webroot, uploadPath);

            var txtUploadPath = _config.GetSection("App:Uploads:txtUploads").Value;
            txtUploadPath = GetFormattedPath(txtUploadPath);
            txtUploadPath = Path.Join(webroot, txtUploadPath);

            var tempUploadsPath = _config.GetSection("App:TempUploads").Value;
            tempUploadsPath = GetFormattedPath(tempUploadsPath);
            tempUploadsPath = Path.Join(webroot, tempUploadsPath);

            var targetFileNamePath = fileName.Replace(tempUploadsPath, uploadPath);
            var targetTxtFileNamePath = txtFileName.Replace(tempUploadsPath, txtUploadPath);

            if (string.IsNullOrEmpty(targetFileNamePath)) throw new Exception($"Error converting PatientTemp.FileName");
            if (string.IsNullOrEmpty(targetTxtFileNamePath)) throw new Exception($"Error converting Patient.TxtFileName");

            result.Add(targetFileNamePath);
            result.Add(targetTxtFileNamePath);

        }
        catch (Exception ex)
        {
            await CustomLogError(ex, $"GetTempFileConversion");
        }
        return result;
    }

    public string GetFormattedPath(string filePath)
    {
        return string.Join(Path.DirectorySeparatorChar, filePath.Split("/"));
    }











}