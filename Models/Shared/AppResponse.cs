using System.Collections.Generic;

namespace Document_Extractor.Models.Shared;
public class AppResult<T>
{
    public AppResult()
    {
        Data = new List<T>();
        Status = false;
    }
    public bool Status { get; set; }
    public string? Message { get; set; }
    public List<T> Data { get; set; }
}
