
namespace Document_Extractor.Models.Shared;

public class UploadRequest 
{
    public UploadRequest()
    {
        
    }

    public IFormFile? Doc { get; set; }
    public long TeamId { get; set; }
}