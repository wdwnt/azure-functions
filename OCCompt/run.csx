#load "..\Shared\NotificationManager.csx"
#load "..\Shared\AirtableHandler.csx"
#load "OCComptItem.csx"
#load "RecordRetriever.csx"
#load "..\Shared\Utility.csx"

using AirtableApiClient;
using HtmlAgilityPack;
using System.Web;

private static TraceWriter _logger;
private static AirtableHandler _airtableHandler;
private static NotificationManager _notificationManager;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    var baseKey = Utility.GetEnvironmentVariable("Airtable_BaseKey");
    var tableName = Utility.GetEnvironmentVariable("Airtable_OCComptTableName");
    _airtableHandler = new AirtableHandler(log, baseKey, tableName);
    _notificationManager = new NotificationManager(log);

    _logger = log;
    _logger.Info($"Start time: {DateTime.Now}");

    var occomptItems = await GetOCCompItems();
    _logger.Info($"Item Count: {occomptItems.Count()}");

    foreach (var item in occomptItems)
    {
        if (!(await _airtableHandler.RecordExists("RecordId", item.Id)))
        {
            _logger.Info($"New result found: {item.Id}");
        
            var fields = new Fields();
            fields.AddField("RecordId", item.Id);
            fields.AddField("Grantor", item.Grantor);
            fields.AddField("Grantee", item.Grantee);
            fields.AddField("ReceivedDate", item.ReceivedDate.ToShortDateString());
            fields.AddField("PdfLink", item.PdfLink);
            await _airtableHandler.AddRecordToTable(fields);

            var message = $"*Orange County Comptroller Update*\nId: {item.Id}\nGrantor: {item.Grantor}\nGrantee: {item.Grantee}\n<{item.PdfLink}|Details>";
            await _notificationManager.SendSlackNotification(message, "tangerine");
        }
    }
}

private static async Task<List<OCComptItem>> GetOCCompItems()
{
    string htmlResults = RecordRetriever.GetSearchResultsAsHtml(DateTime.Now.AddDays(-3), DateTime.Now);
    var htmlDocument = new HtmlDocument();
    htmlDocument.LoadHtml(htmlResults);

    var items = htmlDocument.DocumentNode.SelectNodes("//table[@id='searchResultsTable']//tbody//tr");

    var occompItems = new List<OCComptItem>();
    foreach (var item in items)
    {
        var receivedDateNode = item.SelectSingleNode("//tr[1]/td[2]/a/text()[1]");
        var linkNode = item.SelectSingleNode("//tr[1]/td[3]/a");
        var hrefValue = linkNode.Attributes["href"].Value;

        // Build URL and Get Query Params
        var ub = new UriBuilder(hrefValue);
        var queryString = HttpUtility.ParseQueryString(ub.Query);
        var docName = queryString.Get("docName");
        var attachmentId = queryString.Get("id");
        var parent = queryString.Get("parent");

        // All OCCOmpt Items
        var receivedDate = DateTime.Parse(receivedDateNode.InnerHtml.Replace("&nbsp;", ""));
        var itemId = docName;
        var grantor = item.SelectSingleNode("//tr[1]/td[2]/a/table/tr[1]/td[1]").InnerHtml;
        var grantee = item.SelectSingleNode("//tr[1]/td[2]/a/table/tr[1]/td[2]").InnerHtml;
        var pdfLink = $"http://or.occompt.com/recorder/eagleweb/downloads/{docName}.pdf?id={attachmentId}&parent={parent}";

        var comptItem = new OCComptItem {
            Id = itemId,
            ReceivedDate = receivedDate,
            Grantor = grantor,
            Grantee = grantee,
            PdfLink = pdfLink
        };

        occompItems.Add(comptItem);
    }

    return occompItems;
}
