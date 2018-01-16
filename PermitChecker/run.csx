using System;
using RestSharp;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"SFWMD Executed at: {DateTime.Now}");

    var results = GetSfwmdResults(DateTime.Now, DateTime.Now).ToList();



}

private static IEnumerable<Sfwmd> GetSfwmdResults(DateTime fromDate, DateTime toDate, string permitNumber = "48-00714-P")
{
    try
    {
        var baseClient = new RestClient
        {
            BaseUrl = new Uri(@"http://my.sfwmd.gov"),
            CookieContainer = new CookieContainer()
        };

        var serarchRequest = new RestRequest(Method.POST)
        {
            Resource = "ePermitting/SearchPermit.do"
        };

        var requestBody =
            $"issuingAgency=SFWMD&permitFamilyType=-1&status=-1&applicationNo=&permitNo={permitNumber}" +
            "&projectName=&companyName=&lastName=&county=-1&landuse=-1&township=&range=&ctrLicense=" +
            $"&fromdateDate={fromDate.Day}&fromdateMonth={fromDate.Month}&fromdateYear={fromDate.Year}" +
            $"&todateDate={toDate.Day}&todateMonth={toDate.Month}&todateYear={toDate.Year}" +
            "&startCounter=0&countyName=ALL";

        serarchRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
        serarchRequest.AddParameter("application/x-www-form-urlencoded", requestBody, ParameterType.RequestBody);
        baseClient.Execute(serarchRequest);

        var downloadRequest = new RestRequest(Method.GET)
        {
            Resource = "ePermitting/DownloadResult.do"
        };

        var responseString = baseClient.DownloadData(downloadRequest).AsString();
        return GetSfwmdRecords(responseString.TrimStart());
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return null;
    }
}

private static IEnumerable<Sfwmd> GetSfwmdRecords(string data)
{
    try
    {
        using (var sr = new StringReader(data))
        {
            try
            {
                using (var csv = new CsvReader(sr))
                {
                    csv.Configuration.RegisterClassMap<SfwmdMap>();
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.IgnoreBlankLines = true;
                    csv.Configuration.IsHeaderCaseSensitive = false;
                    csv.Configuration.WillThrowOnMissingField = false;
                    csv.Configuration.TrimFields = true;

                    return csv.GetRecords<Sfwmd>().ToList();
                }
            }
            catch (Exception exception)
            {
                var csvError = exception.Data["CsvHelper"];
                throw new ApplicationException("PAInput CSV File Error: " + csvError);
            }
        }
    }
    catch (CsvMissingFieldException ex)
    {
        throw new ApplicationException("CSV File Error - Missing Field: " + ex.Message);
    }
    catch (Exception ex)
    {
        throw new ApplicationException("CSV File Error: " + ex.Message);
    }
}


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