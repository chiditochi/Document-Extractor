namespace Document_Extractor.Models.DTOs;

public class ExtractDTO
{
    public DateTime? DateTime { get; set; }
    public string OperatorFirstname { get; set; } = string.Empty;
    public string OperatorMiddle { get; set; } = string.Empty;
    public string OperatorSurname { get; set; } = string.Empty;
    public string PatientNHS { get; set; } = string.Empty;
    public string PatientTitle { get; set; } = string.Empty;
    public string PatientFirstname { get; set; } = string.Empty;
    public string PatientMiddle { get; set; } = string.Empty;
    public string PatientSurname { get; set; } = string.Empty;
    public DateTime? PatientDOB { get; set; }
    public string PatientSex { get; set; } = string.Empty;
    public string PatientSexCode { get; set; } = string.Empty;
    public string PatientHousename { get; set; } = string.Empty;
    public string PatientAddress1 { get; set; } = string.Empty;
    public string PatientAddress2 { get; set; } = string.Empty;
    public string PatientAddress3 { get; set; } = string.Empty;
    public string PatientAddress4 { get; set; } = string.Empty;
    public string PatientPostcode { get; set; } = string.Empty;
    public string PatientPhoneno { get; set; } = string.Empty;
    public string PatientReligion { get; set; } = string.Empty;
    public string PatientEthnicity { get; set; } = string.Empty;
    public string PatientPractice { get; set; } = string.Empty;
    public string PatientPracticeAddress { get; set; } = string.Empty;
    public string PatientPracticeCode { get; set; } = string.Empty;
    public string PatientGPTitle { get; set; } = string.Empty;
    public string PatientGPFirstName { get; set; } = string.Empty;
    public string PatientGPSurname { get; set; } = string.Empty;
    public string PatientGPCode { get; set; } = string.Empty;


}