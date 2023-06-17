
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Document_Extractor.Services.Interfaces;

public interface IHelperService
{
    public Task CustomLogError(Exception e, string action);
    public Task<bool> DeleteFile(string filePath);
    public Task<bool> DeleteFiles(string[] filePaths);
    public List<string> GetModelStateErrors(ModelStateDictionary modelState);

}