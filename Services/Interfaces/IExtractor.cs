
using Document_Extractor.Models.Shared;

namespace Document_Extractor.Services.Interfaces;

public interface IExtractor
{
    public Task<AppResult<string>> ReadFile(string filePath);
}