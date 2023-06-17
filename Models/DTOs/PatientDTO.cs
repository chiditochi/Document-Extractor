namespace Document_Extractor.Models.DB
{
    public class PatientDTO
    {
        public PatientDTO()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public long PatientId { get; set; }
        public DateTime DateTime { get; set; }
        public string? OperatorFirstname { get; set; }
        public string? OperatorMiddle { get; set; }
        public string? OperatorSurname { get; set; }
        public string? PatientNHS { get; set; }
        public string? PatientTitle { get; set; }
        public string? PatientFirstname { get; set; }
        public string? PatientMiddle { get; set; }
        public string? PatientSurname { get; set; }
        public DateTime PatientDOB { get; set; }
        public string? PatientSex { get; set; }
        public string? PatientSexCode { get; set; }
        public string? PatientHousename { get; set; }
        public string? PatientAddress1 { get; set; }
        public string? PatientAddress2 { get; set; }
        public string? PatientAddress3 { get; set; }
        public string? PatientAddress4 { get; set; }
        public string? PatientPostcode { get; set; }
        public string? PatientPhoneno { get; set; }
        public string? PatientReligion { get; set; }
        public string? PatientEthnicity { get; set; }
        public string? PatientPractice { get; set; }
        public string? PatientPracticeAddress { get; set; }
        public string? PatientPracticeCode { get; set; }
        public string? PatientGPTitle { get; set; }
        public string? PatientGPFirstName { get; set; }
        public string? PatientGPSurname { get; set; }
        public string? PatientGPCode { get; set; }



        public long TeamId { get; set; }
        public bool IsUploadComfirmed { get; set; }
        public bool Status { get; set; }
        public string? FileName1 { get; set; }
        public string? FileName2 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public virtual TeamDTO Team { get; set; } = new TeamDTO();

    }
}
