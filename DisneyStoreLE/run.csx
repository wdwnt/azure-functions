#load "..\Shared\NotificationManager.csx"
#load "MerchandiseItem.csx"

using HtmlAgilityPack;

private static TraceWriter _logger;
private static NotificationManager _notificationManager;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    _logger = log;
    _notificationManager = new NotificationManager(log);

    const string url = "https://www.shopdisney.com/disney-parks-limited-release-items";
    var htmlWeb = new HtmlWeb();
    var htmlDocument = htmlWeb.Load(url);
    var items = htmlDocument.DocumentNode.SelectNodes("//ul[@class='items-container']//li");

    foreach (var item in items)
    {
        var titleDiv = item.SelectSingleNode(".//div[@class='title']");

        var merchandiseItem = new MerchandiseItem
        {
            Id = Int32.Parse(titleDiv.Attributes["id"].Value.Replace("-title", String.Empty)),
            Name = titleDiv.InnerText,
            Price = item.SelectSingleNode(".//span[@class='price listprice']").InnerText
        };

        _logger.Info(merchandiseItem.ToString());
        //await _notificationManager.SendSlackNotification(merchandiseItem.ToString(), "shirt");
    }
}
