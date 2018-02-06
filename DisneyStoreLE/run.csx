#load "..\Shared\AirtableHandler.csx"
#load "MerchandiseItem.csx"
#load "..\Shared\NotificationManager.csx"

using AirtableApiClient;
using HtmlAgilityPack;

private static TraceWriter _logger;
private static AirtableHandler _airtableHandler;
private static NotificationManager _notificationManager;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    var baseKey = Utility.GetEnvironmentVariable("Airtable_MerchandiseBaseKey");
    var tableName = Utility.GetEnvironmentVariable("Airtable_MerchandiseTableName");
    _airtableHandler = new AirtableHandler(log, baseKey, tableName);
    _notificationManager = new NotificationManager(log);

    _logger = log;
    _logger.Info($"Start time: {DateTime.Now}");

    var merchandiseItems = await GetMerchandiseItems();
    _logger.Info($"Items Found: {merchandiseItems.Count}");

    foreach (var item in merchandiseItems)
    {
        _logger.Info($"Item: {item.Name}");
        
        if (!(await _airtableHandler.RecordExists("Id", item.Id.ToString())))
        {
            _logger.Info($"New result found: {item.Id}");

            var fields = new Fields();
            fields.AddField("Id", item.Id);
            fields.AddField("Name", item.Name);
            fields.AddField("Description", item.Description);
            fields.AddField("DetailsUrl", item.DetailsUrl);
            fields.AddField("Price", item.Price);
            await _airtableHandler.AddRecordToTable(fields);

            var message = $"*shopDisney Limited Edition Item*\n*Name:* {item.Name}\n*Price:* {item.Price}\n<https://www.shopdisney.com{item.DetailsUrl}|Details>";
            await _notificationManager.SendSlackNotification(message, "shirt");
        }
    }
}

private static async Task<List<MerchandiseItem>> GetMerchandiseItems()
{
    const string url = "https://www.shopdisney.com/disney-parks-limited-release-items";
    var htmlWeb = new HtmlWeb();
    var htmlDocument = htmlWeb.Load(url);
    var items = htmlDocument.DocumentNode.SelectNodes("//ul[@class='items-container']//li");

    var merchandiseItems = new List<MerchandiseItem>();

    foreach (var item in items)
    {
        var titleDiv = item.SelectSingleNode(".//div[@class='title']");

        var merchandiseItem = new MerchandiseItem
        {
            Id = Int32.Parse(titleDiv.Attributes["id"].Value.Replace("-title", String.Empty)),
            Name = titleDiv.InnerText,
            DetailsUrl = item.SelectSingleNode(".//a[@class='ada-el-focus']").Attributes["href"].Value,
            Price = item.SelectSingleNode(".//span[@class='price listprice']").InnerText
        };

        merchandiseItems.Add(merchandiseItem);
    }

    return merchandiseItems;
}
