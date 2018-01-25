using CsvHelper;
using CsvHelper.Configuration;

public sealed class SfwmdMap : CsvClassMap<Sfwmd>
{
    public SfwmdMap()
    {
        Map(m => m.ApplicationNum).Index(0).Name("APPLICATION NO");
        Map(m => m.PermitNumber).Index(1).Name("PERMIT NO");
        Map(m => m.ApprovedDate).Index(2).Name("APPROVED DATE");
        Map(m => m.ReceivedDate).Index(3).Name("RECEIVED DATE");
        Map(m => m.Status).Index(4).Name("STATUS");
        Map(m => m.PermitType).Index(5).Name("PERMIT TYPE");
        Map(m => m.PermitStatus).Index(6).Name("PERMIT_STATUS");
        Map(m => m.ExpirationDate).Index(7).Name("EXPIRATION DATE");
        Map(m => m.ProjectAcres).Index(8).Name("PROJECT ACRES");
        Map(m => m.ProjectName).Index(9).Name("PROJECT NAME");
        Map(m => m.Deadline).Index(10).Name("DEADLINE");
        Map(m => m.WaterSourceReceivingBody).Index(11).Name("WATERSOURCE/RECEIVING BODY");
        Map(m => m.County).Index(12).Name("COUNTY");
        Map(m => m.Location).Index(13).Name("LOCATION");
        Map(m => m.LandUses).Index(14).Name("LANDUSES");
        Map(m => m.Reviewer).Index(15).Name("REVIEWER");
        Map(m => m.PartyOfConcerns).Index(16).Name("PARTY OF CONCERNS");
        Map(m => m.FileLocation).Index(17).Name("FILE LOCATION");
        Map(m => m.LettersAndResponse).Index(18).Name("LETTERS AND RESPONSE");
    }
}
