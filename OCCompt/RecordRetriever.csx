using RestSharp;
using RestSharp.Extensions;
using System.Net;

public static class RecordRetriever
{
    public static string GetSearchResultsAsHtml(DateTime startDateTime, DateTime endDateTime)
    {
        // Get Cookies
        IList<RestResponseCookie> responseCookies = GetCookies();

        // Setup Client
        RestClient baseClient = new RestClient
        {
            BaseUrl = new Uri(@"http://or.occompt.com"),
        };

        // Set Cookies from First Hit
        CookieContainer cookieJar = new CookieContainer();
        foreach (RestResponseCookie cookie in responseCookies)
        {
            cookieJar.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
        }
        baseClient.CookieContainer = cookieJar;

        // Create Search Request
        RestRequest searchRequest = new RestRequest(Method.POST)
        {
            Resource = "recorder/eagleweb/docSearchPOST.jsp"
        };

        string startDate = WebUtility.UrlEncode(startDateTime.ToString("d"));
        string endDate = WebUtility.UrlEncode(endDateTime.ToString("d"));

        string requestBody = $"RecordingDateIDStart={startDate}&RecordingDateIDEnd={endDate}&BothNamesIDSearchString=disney" +
                                    "&BothNamesIDSearchType=Wildcard+Search&GrantorIDSearchString=&GrantorIDSearchType=Exact+Match&GranteeIDSearchString=" +
                                    "&GranteeIDSearchType=Exact+Match&DocumentID=&BookPageIDBook=&BookPageIDPage=&PLSSIDSixtyFourthSection=&PLSSIDSection=" +
                                    "&PLSSIDTownship=&PLSSIDRange=&PlattedIDLot=&PlattedIDBlock=&PlattedIDTract=&PlattedIDUnit=&CaseID=&DeedDocTaxStart=" +
                                    "&DeedDocTaxEnd=&MortgageDocTaxStart=&MortgageDocTaxEnd=&IntangibleTaxStart=&IntangibleTaxEnd=&ParcelID=&LegalRemarks=" +
                                    "&docTypeTotal=41&__search_select=NC";

        searchRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
        searchRequest.AddParameter("application/x-www-form-urlencoded", requestBody, ParameterType.RequestBody);

        IRestResponse response = baseClient.Execute(searchRequest);
        if (response.ResponseStatus == ResponseStatus.Completed)
        {
            return response.StatusCode == HttpStatusCode.OK ? response.Content : null;
        }
        return null;
    }

    private static IList<RestResponseCookie> GetCookies()
    {
        RestClient baseClient = new RestClient
        {
            BaseUrl = new Uri(@"http://or.occompt.com"),
            CookieContainer = new CookieContainer(),
            FollowRedirects = false
        };
        RestRequest loginRequest = new RestRequest(Method.GET) {
            Resource = "recorder/web/loginPOST.jsp",
        };

        loginRequest.Parameters.Add(new Parameter { Name = "submit", Type = ParameterType.QueryString, Value = "I%2bAcknowledge" });
        loginRequest.Parameters.Add(new Parameter { Name = "guest", Type = ParameterType.QueryString, Value = "true" });
        
        IRestResponse response = baseClient.Execute(loginRequest);
        
        return response.Cookies;

    }

    
}
