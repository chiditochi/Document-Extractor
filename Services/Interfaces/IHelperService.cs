
namespace Document_Extractor.Services.Interfaces;

public interface IHelperService
{
    public Task CustomLogError(Exception e, string action);
    public Task<bool> DeleteFile(string filePath);
    public Task<bool> DeleteFiles(string[] filePaths);

}