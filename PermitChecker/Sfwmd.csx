using CsvHelper;
using CsvHelper.Configuration;

public class Sfwmd
{
    public string ApplicationNum { get; set; }
    public string PermitNumber { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string IssuingOffice { get; set; }
    public string Status { get; set; }
    public string PermitType { get; set; }
    public string PermitStatus { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string ProjectAcres { get; set; }
    public string ProjectName { get; set; }
    public DateTime Deadline { get; set; }
    public string WaterSourceReceivingBody { get; set; }
    public string County { get; set; }
    public string Location { get; set; }
    public string LandUses { get; set; }
    public string Reviewer { get; set; }
    public string PartyOfConcerns { get; set; }
    public string FileLocation { get; set; }
    public string LettersAndResponse { get; set; }
}
