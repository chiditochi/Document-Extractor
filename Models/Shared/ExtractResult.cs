using Document_Extractor.Models.DTOs;

namespace Document_Extractor.Models.Shared;


public class ExtractResult
{
    public ExtractResult()
    {
        Data = new ExtractDTO();
    }
    public ExtractResult(string storageName, string storageName2, ExtractDTO data)
    {
        StorageName = storageName;
        StorageName2 = storageName2;
        Data = data;
    }
    public string StorageName { get; set; } = string.Empty;
    public string StorageName2 { get; set; } = string.Empty;
    public ExtractDTO Data { get; set; }
}