using CsvHelper;
using CsvHelper.Configuration;
using RestSharp;
using RestSharp.Extensions;
using System.Net;

public static class RecordRetriever
{
    public static IEnumerable<Sfwmd> GetSfwmdResults(DateTime fromDate, DateTime toDate, string permitNumber = "48-00714-P")
    {
        try
        {
            var baseClient = new RestClient
            {
                BaseUrl = new Uri(@"http://my.sfwmd.gov"),
                CookieContainer = new CookieContainer()
            };

            var searchRequest = new RestRequest(Method.POST)
            {
                Resource = "ePermitting/SearchPermit.do"
            };

            var requestBody =
                $"issuingAgency=SFWMD&permitFamilyType=-1&status=-1&applicationNo=&permitNo={permitNumber}" +
                "&projectName=&companyName=&lastName=&county=-1&landuse=-1&township=&range=&ctrLicense=" +
                $"&fromdateDate={fromDate.Day}&fromdateMonth={fromDate.Month}&fromdateYear={fromDate.Year}" +
                $"&todateDate={toDate.Day}&todateMonth={toDate.Month}&todateYear={toDate.Year}" +
                "&startCounter=0&countyName=ALL";

            searchRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            searchRequest.AddParameter("application/x-www-form-urlencoded", requestBody, ParameterType.RequestBody);
            baseClient.Execute(searchRequest);

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
}
