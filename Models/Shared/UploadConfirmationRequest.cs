
namespace Document_Extractor.Models.Shared;

public class UploadConfirmationRequest
{
    public UploadConfirmationRequest()
    {
        
    }

    public long PatientId { get; set; }
    public bool Status { get; set; }
}