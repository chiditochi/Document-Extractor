
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Document_Extractor.Services.Interfaces;

public interface IHelperService
{
    public Task CustomLogError(Exception e, string action);
    public Task<bool> DeleteFile(string filePath);
    public Task<bool> DeleteFiles(string[] filePaths);
    public List<string> GetModelStateErrors(ModelStateDictionary modelState);
    public Task<bool> CreateFolder(string filePath);
    public Task<bool> CleanupTempUploadFiles(string fileName, string txtFileName);
    public Task<bool> CopyFilesToConfiguredFolders(string fileName, string txtFileName, List<string> convertedFileNames);
    public Task<List<string>> GetTempFileConversion(string fileName, string txtFileName);


    public Task<bool> CreateUploadFolders();
    public string GetFormattedPath(string filePath);


}