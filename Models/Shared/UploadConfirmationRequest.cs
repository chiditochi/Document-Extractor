
namespace Document_Extractor.Models.Shared;

public class UploadConfirmationRequest
{
    public UploadConfirmationRequest()
    {
        
    }

    public long PatientTempId { get; set; }
    public bool Status { get; set; }
}